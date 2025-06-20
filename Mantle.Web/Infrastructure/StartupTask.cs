﻿using Mantle.Security;
using Mantle.Tenants.Entities;

namespace Mantle.Web.Infrastructure;

public class StartupTask : IStartupTask
{
    #region IStartupTask Members

    public void Execute()
    {
        EnsureTenant();

        var tenantService = DependoResolver.Instance.Resolve<ITenantService>();
        IEnumerable<int> tenantIds = null;

        using (var connection = tenantService.OpenConnection())
        {
            tenantIds = connection.Query().Select(x => x.Id).ToList();
        }

        var membershipService = DependoResolver.Instance.Resolve<IMembershipService>();

        AsyncHelper.RunSync(() => EnsurePermissions(membershipService, tenantIds));
        AsyncHelper.RunSync(() => EnsureMembership(membershipService, tenantIds));

        EnsureSettings(tenantIds);
    }

    public int Order => 1;

    #endregion IStartupTask Members

    private static void EnsureTenant()
    {
        var tenantService = DependoResolver.Instance.Resolve<ITenantService>();

        if (tenantService.Count() == 0)
        {
            tenantService.Insert(new Tenant
            {
                Name = "Default",
                Url = "my-domain.com",
                Hosts = "my-domain.com"
            });
        }
    }

    private static async Task EnsurePermissions(IMembershipService membershipService, IEnumerable<int> tenantIds)
    {
        if (membershipService.SupportsRolePermissions)
        {
            #region NULL Tenant

            var permissionProviders = DependoResolver.Instance.ResolveAll<IPermissionProvider>();

            var allPermissions = permissionProviders.SelectMany(x => x.GetPermissions());
            var allPermissionNames = allPermissions.Select(x => x.Name).ToHashSet();

            var installedPermissions = await membershipService.GetAllPermissions(null);
            var installedPermissionNames = installedPermissions.Select(x => x.Name).ToHashSet();

            var permissionsToAdd = allPermissions
                .Where(x => !installedPermissionNames.Contains(x.Name))
                .Select(x => new MantlePermission
                {
                    Name = x.Name,
                    TenantId = null,
                    Category = x.Category,
                    Description = x.Description
                })
                .OrderBy(x => x.Category)
                .ThenBy(x => x.Name);

            if (!permissionsToAdd.IsNullOrEmpty())
            {
                await membershipService.InsertPermissions(permissionsToAdd);
            }

            var permissionIdsToDelete = installedPermissions
                .Where(x => !allPermissionNames.Contains(x.Name))
                .Select(x => x.Id);

            if (!permissionIdsToDelete.IsNullOrEmpty())
            {
                await membershipService.DeletePermissions(permissionIdsToDelete);
            }

            #endregion NULL Tenant

            #region Tenants

            foreach (int tenantId in tenantIds)
            {
                installedPermissions = await membershipService.GetAllPermissions(tenantId);
                installedPermissionNames = installedPermissions.Select(x => x.Name).ToHashSet();

                permissionsToAdd = allPermissions
                   .Where(x => !installedPermissionNames.Contains(x.Name))
                   .Select(x => new MantlePermission
                   {
                       TenantId = tenantId,
                       Name = x.Name,
                       Category = x.Category,
                       Description = x.Description
                   })
                   .OrderBy(x => x.Category)
                   .ThenBy(x => x.Name);

                if (!permissionsToAdd.IsNullOrEmpty())
                {
                    await membershipService.InsertPermissions(permissionsToAdd);
                }

                permissionIdsToDelete = installedPermissions
                   .Where(x => !allPermissionNames.Contains(x.Name))
                   .Select(x => x.Id);

                if (!permissionIdsToDelete.IsNullOrEmpty())
                {
                    await membershipService.DeletePermissions(permissionIdsToDelete);
                }
            }

            #endregion Tenants
        }
    }

    private static async Task EnsureMembership(IMembershipService membershipService, IEnumerable<int> tenantIds)
    {
        // We only run this method to ensure that the admin user has been setup as part of the installation process.
        //  If there are any users already in the DB...
        if ((await membershipService.GetAllUsers(null)).Any())
        {
            // ... we assume the admin user is one of them. No need for further querying...
            return;
        }

        var dataSettings = DependoResolver.Instance.Resolve<DataSettings>();

        var adminUser = await membershipService.GetUserByEmail(null, dataSettings.AdminEmail);
        if (adminUser == null)
        {
            await membershipService.InsertUser(
                new MantleUser
                {
                    TenantId = null,
                    UserName = dataSettings.AdminEmail,
                    Email = dataSettings.AdminEmail
                },
                dataSettings.AdminPassword);

            adminUser = await membershipService.GetUserByEmail(null, dataSettings.AdminEmail);

            // TODO: This doesn't work. Gets error like "No owin.Environment item was found in the context."
            //// Confirm User
            //string token = await membershipService.GenerateEmailConfirmationToken(adminUser.Id);
            //await membershipService.ConfirmEmail(adminUser.Id, token);

            MantleRole administratorsRole = null;
            if (adminUser != null)
            {
                administratorsRole = await membershipService.GetRoleByName(null, MantleSecurityConstants.Roles.Administrators);
                if (administratorsRole == null)
                {
                    await membershipService.InsertRole(new MantleRole
                    {
                        TenantId = null,
                        Name = MantleSecurityConstants.Roles.Administrators
                    });
                    administratorsRole = await membershipService.GetRoleByName(null, MantleSecurityConstants.Roles.Administrators);
                    await membershipService.AssignUserToRoles(null, adminUser.Id, new[] { administratorsRole.Id });
                }
            }

            if (membershipService.SupportsRolePermissions && administratorsRole != null)
            {
                var fullAccessPermission = await membershipService.GetPermissionByName(null, StandardPermissions.FullAccess.Name);
                await membershipService.AssignPermissionsToRole(administratorsRole.Id, new[] { fullAccessPermission.Id });
            }

            dataSettings.AdminPassword = null;
            DataSettingsManager.SaveSettings(dataSettings);
        }

        if (membershipService.SupportsRolePermissions)
        {
            foreach (int tenantId in tenantIds)
            {
                await membershipService.EnsureAdminRoleForTenant(tenantId);
            }
        }
    }

    private static void EnsureSettings(IEnumerable<int> tenantIds)
    {
        var settingsRepository = DependoResolver.Instance.Resolve<IRepository<Setting>>();
        var allSettings = DependoResolver.Instance.ResolveAll<ISettings>();
        var allSettingNames = allSettings.Select(x => x.Name).ToList();

        #region NULL Tenant (In case we want default settings)

        var installedSettings = settingsRepository.Find(new SearchOptions<Setting>
        {
            Query = x => x.TenantId == null
        });
        var installedSettingNames = installedSettings.Select(x => x.Name).ToList();

        var settingsToAdd = allSettings
            .Where(x => x.IsTenantRestricted && !installedSettingNames.Contains(x.Name))
            .Select(x => new Setting
            {
                Id = Guid.NewGuid(),
                TenantId = null,
                Name = x.Name,
                Type = x.GetType().FullName,
                Value = Activator.CreateInstance(x.GetType()).JsonSerialize()
            })
            .ToList();

        if (!settingsToAdd.IsNullOrEmpty())
        {
            settingsRepository.Insert(settingsToAdd);
        }

        var settingsToDelete = installedSettings.Where(x => !allSettingNames.Contains(x.Name)).ToList();

        if (!settingsToDelete.IsNullOrEmpty())
        {
            settingsRepository.Delete(settingsToDelete);
        }

        #endregion NULL Tenant (In case we want default settings)

        #region Tenants

        foreach (int tenantId in tenantIds)
        {
            installedSettings = settingsRepository.Find(new SearchOptions<Setting>
            {
                Query = x => x.TenantId == tenantId
            });
            installedSettingNames = installedSettings.Select(x => x.Name).ToList();

            settingsToAdd = allSettings
                .Where(x => !x.IsTenantRestricted && !installedSettingNames.Contains(x.Name))
                .Select(x => new Setting
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = x.Name,
                    Type = x.GetType().FullName,
                    Value = Activator.CreateInstance(x.GetType()).JsonSerialize()
                })
                .ToList();

            if (!settingsToAdd.IsNullOrEmpty())
            {
                settingsRepository.Insert(settingsToAdd);
            }

            settingsToDelete = installedSettings.Where(x => !allSettingNames.Contains(x.Name)).ToList();

            if (!settingsToDelete.IsNullOrEmpty())
            {
                settingsRepository.Delete(settingsToDelete);
            }
        }

        #endregion Tenants
    }
}