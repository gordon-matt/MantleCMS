using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mantle;
using Mantle.Infrastructure;
using Mantle.Security;
using Mantle.Security.Membership;
using Mantle.Security.Membership.Permissions;
using Mantle.Tenants.Services;
using Mantle.Threading;

namespace MantleCMS.Infrastructure
{
    // NOTE: This class is temporary, until installation page is done.

    // Can't use this as normal IStartupTask, because Current Tenant is NULL when app starts up.. need to insert user from HomeControlle or other place when app starts
    //public class StartupTask : IStartupTask
    public static class StartupTask
    {
        public static void Execute()
        {
            var tenantService = EngineContext.Current.Resolve<ITenantService>();
            IEnumerable<int> tenantIds = null;

            using (var connection = tenantService.OpenConnection())
            {
                tenantIds = connection.Query().Select(x => x.Id).ToList();
            }

            var membershipService = EngineContext.Current.Resolve<IMembershipService>();
            AsyncHelper.RunSync(() => EnsureMembership(membershipService, tenantIds));
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

            var adminUser = await membershipService.GetUserByEmail(null, "admin@yourSite.com");
            if (adminUser == null)
            {
                await membershipService.InsertUser(
                    new MantleUser
                    {
                        TenantId = null,
                        UserName = "admin@yourSite.com",
                        Email = "admin@yourSite.com"
                    },
                    "Admin@123");

                adminUser = await membershipService.GetUserByEmail(null, "admin@yourSite.com");

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
            }

            if (membershipService.SupportsRolePermissions)
            {
                foreach (int tenantId in tenantIds)
                {
                    await membershipService.EnsureAdminRoleForTenant(tenantId);
                }
            }
        }
    }
}