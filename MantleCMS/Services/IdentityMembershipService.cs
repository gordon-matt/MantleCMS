using Extenso.Collections;
using Extenso.Data.Entity;
using Mantle.Exceptions;
using Mantle.Security;
using Mantle.Security.Membership;
using Mantle.Web;
using Mantle.Web.Security.Membership;
using MantleCMS.Data.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MantleCMS.Services
{
    public abstract class IdentityMembershipService : IMembershipService
    {
        private readonly IDbContextFactory contextFactory;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IRepository<UserProfileEntry> userProfileRepository;

        private static Dictionary<string, List<MantleRole>> cachedUserRoles;
        private static Dictionary<string, List<MantlePermission>> cachedRolePermissions;

        static IdentityMembershipService()
        {
            cachedUserRoles = new Dictionary<string, List<MantleRole>>();
            cachedRolePermissions = new Dictionary<string, List<MantlePermission>>();
        }

        public IdentityMembershipService(
            IDbContextFactory contextFactory,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IRepository<UserProfileEntry> userProfileRepository)
        {
            this.contextFactory = contextFactory;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.userProfileRepository = userProfileRepository;
        }

        #region IMembershipService Members

        public bool SupportsRolePermissions => true;

        public async Task<string> GenerateEmailConfirmationToken(object userId)
        {
            string id = userId.ToString();
            var user = await userManager.FindByIdAsync(id);
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task ConfirmEmail(object userId, string token)
        {
            string id = userId.ToString();
            var user = await userManager.FindByIdAsync(id);
            await userManager.ConfirmEmailAsync(user, token);
        }

        #region Users

        // Ignore this.. it was used for OData in MVC5 and will be again in future...
        private IQueryable<MantleUser> GetAllUsersAsQueryable(DbContext context, int? tenantId)
        {
            IQueryable<ApplicationUser> query = context.Set<ApplicationUser>();

            if (tenantId.HasValue)
            {
                query = query.Where(x => x.TenantId == tenantId);
            }
            else
            {
                query = query.Where(x => x.TenantId == null);
            }

            return query
                .Select(x => new MantleUser
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    UserName = x.UserName,
                    Email = x.Email,
                    IsLockedOut = x.LockoutEnabled
                });
        }

        public async Task<IEnumerable<MantleUser>> GetAllUsers(int? tenantId)
        {
            using (var context = contextFactory.GetContext())
            {
                return await GetAllUsersAsQueryable(context, tenantId).ToHashSetAsync();
            }
        }

        public async Task<IEnumerable<MantleUser>> GetUsers(int? tenantId, Expression<Func<MantleUser, bool>> predicate)
        {
            using (var context = contextFactory.GetContext())
            {
                return await GetAllUsersAsQueryable(context, tenantId)
                    .Where(predicate)
                    .ToHashSetAsync();
            }
        }

        public async Task<MantleUser> GetUserById(object userId)
        {
            string id = userId.ToString();
            //var user = userManager.FindById(id);

            using (var context = contextFactory.GetContext())
            {
                var user = context.Set<ApplicationUser>().Find(userId);

                if (user == null)
                {
                    return null;
                }

                return await Task.FromResult(new MantleUser
                {
                    Id = user.Id,
                    TenantId = user.TenantId,
                    UserName = user.UserName,
                    Email = user.Email,
                    IsLockedOut = user.LockoutEnabled
                });
            }
        }

        public async Task<MantleUser> GetUserByEmail(int? tenantId, string email)
        {
            ApplicationUser user;

            using (var context = contextFactory.GetContext())
            {
                if (tenantId.HasValue)
                {
                    user = await context.Set<ApplicationUser>().FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Email == email);
                }
                else
                {
                    user = await context.Set<ApplicationUser>().FirstOrDefaultAsync(x => x.TenantId == null && x.Email == email);
                }

                if (user == null)
                {
                    return null;
                }

                return new MantleUser
                {
                    Id = user.Id,
                    TenantId = user.TenantId,
                    UserName = user.UserName,
                    Email = user.Email,
                    IsLockedOut = user.LockoutEnabled
                };
            }
        }

        public async Task<MantleUser> GetUserByName(int? tenantId, string userName)
        {
            ApplicationUser user;

            using (var context = contextFactory.GetContext())
            {
                if (tenantId.HasValue)
                {
                    user = await context.Set<ApplicationUser>().FirstOrDefaultAsync(x => x.TenantId == tenantId && x.UserName == userName);
                }
                else
                {
                    user = await context.Set<ApplicationUser>().FirstOrDefaultAsync(x => x.TenantId == null && x.UserName == userName);
                }

                if (user == null)
                {
                    return null;
                }

                return new MantleUser
                {
                    Id = user.Id,
                    TenantId = user.TenantId,
                    UserName = user.UserName,
                    Email = user.Email,
                    IsLockedOut = user.LockoutEnabled
                };
            }
        }

        public async Task<IEnumerable<MantleRole>> GetRolesForUser(object userId)
        {
            string id = userId.ToString();
            if (cachedUserRoles.ContainsKey(id))
            {
                return cachedUserRoles[id];
            }

            var user = await userManager.FindByIdAsync(id);
            var roleNames = await userManager.GetRolesAsync(user);

            var roles = new List<MantleRole>();
            foreach (string roleName in roleNames)
            {
                var superRole = await GetRoleByName(null, roleName);
                if (superRole != null)
                {
                    roles.Add(new MantleRole
                    {
                        Id = superRole.Id,
                        TenantId = null,
                        Name = superRole.Name
                    });
                }

                var tenantRole = await GetRoleByName(user.TenantId, roleName);
                if (tenantRole != null)
                {
                    roles.Add(new MantleRole
                    {
                        Id = tenantRole.Id,
                        TenantId = null,
                        Name = tenantRole.Name
                    });
                }
            }

            cachedUserRoles.Add(id, roles);
            return roles;
        }

        public async Task<bool> DeleteUser(object userId)
        {
            string id = userId.ToString();
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                return result.Succeeded;
            }

            return false;
        }

        public async Task InsertUser(MantleUser user, string password)
        {
            // Check for spaces in UserName above, because of this:
            // http://stackoverflow.com/questions/30078332/bug-in-asp-net-identitys-usermanager
            string userName = (user.UserName.Contains(" ") ? user.UserName.Replace(" ", "_") : user.UserName);

            var appUser = new ApplicationUser
            {
                TenantId = user.TenantId,
                UserName = userName,
                Email = user.Email,
                LockoutEnabled = user.IsLockedOut
            };

            var result = await userManager.CreateAsync(appUser, password);

            if (!result.Succeeded)
            {
                string errorMessage = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
                throw new MantleException(errorMessage);
            }
        }

        public async Task UpdateUser(MantleUser user)
        {
            string userId = user.Id.ToString();
            var existingUser = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user != null)
            {
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.LockoutEnabled = user.IsLockedOut;
                var result = await userManager.UpdateAsync(existingUser);

                if (!result.Succeeded)
                {
                    string errorMessage = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
                    throw new MantleException(errorMessage);
                }
            }
        }

        //public async Task AssignUserToRoles(object userId, IEnumerable<object> roleIds)
        //{
        //    string uId = userId.ToString();

        //    var ids = roleIds.Select(x => Convert.ToString(x));
        //    var roleNames = await roleManager.Roles.Where(x => ids.Contains(x.Id)).Select(x => x.Name).ToListAsync();

        //    var currentRoles = await userManager.GetRolesAsync(uId);

        //    var toDelete = currentRoles.Where(x => !roleNames.Contains(x));
        //    var toAdd = roleNames.Where(x => !currentRoles.Contains(x));

        //    if (toDelete.Any())
        //    {
        //        foreach (string roleName in toDelete)
        //        {
        //            var result = await userManager.RemoveFromRoleAsync(uId, roleName);

        //            if (!result.Succeeded)
        //            {
        //                string errorMessage = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
        //                throw new MantleException(errorMessage);
        //            }
        //        }
        //        cachedUserRoles.Remove(uId);
        //    }

        //    if (toAdd.Any())
        //    {
        //        foreach (string roleName in toAdd)
        //        {
        //            var result = await userManager.AddToRoleAsync(uId, roleName);

        //            if (!result.Succeeded)
        //            {
        //                string errorMessage = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
        //                throw new MantleException(errorMessage);
        //            }
        //        }
        //        cachedUserRoles.Remove(uId);
        //    }
        //}

        public async Task AssignUserToRoles(int? tenantId, object userId, IEnumerable<object> roleIds)
        {
            string uId = userId.ToString();

            IQueryable<string> currentRoleIds;

            using (var context = contextFactory.GetContext())
            {
                if (tenantId.HasValue)
                {
                    currentRoleIds = from ur in context.Set<IdentityUserRole<string>>()
                                     join r in context.Set<ApplicationRole>() on ur.RoleId equals r.Id
                                     where r.TenantId == tenantId && ur.UserId == uId
                                     select ur.RoleId;
                }
                else
                {
                    currentRoleIds = from ur in context.Set<IdentityUserRole<string>>()
                                     join r in context.Set<ApplicationRole>() on ur.RoleId equals r.Id
                                     where r.TenantId == null && ur.UserId == uId
                                     select ur.RoleId;
                }

                var rIds = roleIds.ToListOf<string>();

                var toDelete = from ur in context.Set<IdentityUserRole<string>>()
                               join r in context.Set<ApplicationRole>() on ur.RoleId equals r.Id
                               where r.TenantId == tenantId
                               && ur.UserId == uId
                               && !rIds.Contains(ur.RoleId)
                               select ur;

                var toAdd = rIds.Where(x => !currentRoleIds.Contains(x)).Select(x => new IdentityUserRole<string>
                {
                    UserId = uId,
                    RoleId = x
                });

                if (toDelete.Any())
                {
                    context.Set<IdentityUserRole<string>>().RemoveRange(toDelete);
                }

                if (toAdd.Any())
                {
                    context.Set<IdentityUserRole<string>>().AddRange(toAdd);
                }

                await context.SaveChangesAsync();
            }

            cachedUserRoles.Remove(uId);
        }

        public async Task ChangePassword(object userId, string newPassword)
        {
            //TODO: This doesn't seem to be working very well; no errors, but can't login with the given password
            string id = userId.ToString();
            var user = await userManager.FindByIdAsync(id);
            var result = await userManager.RemovePasswordAsync(user);

            if (!result.Succeeded)
            {
                string errorMessage = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
                throw new MantleException(errorMessage);
            }

            result = await userManager.AddPasswordAsync(user, newPassword);

            if (!result.Succeeded)
            {
                string errorMessage = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
                throw new MantleException(errorMessage);
            }
            //var user = userManager.FindById(id);
            //string passwordHash = userManager.PasswordHasher.HashPassword(newPassword);
            //userStore.SetPasswordHashAsync(user, passwordHash);
            //userManager.UpdateSecurityStamp(id);
        }

        public async Task<string> GetUserDisplayName(MantleUser user)
        {
            var profile = await GetProfile(user.Id);

            bool hasFamilyName = profile.ContainsKey(AccountUserProfileProvider.Fields.FamilyName);
            bool hasGivenNames = profile.ContainsKey(AccountUserProfileProvider.Fields.GivenNames);

            if (hasFamilyName && hasGivenNames)
            {
                string familyName = profile[AccountUserProfileProvider.Fields.FamilyName];
                string givenNames = profile[AccountUserProfileProvider.Fields.GivenNames];

                if (profile.ContainsKey(AccountUserProfileProvider.Fields.ShowFamilyNameFirst))
                {
                    bool showFamilyNameFirst = bool.Parse(profile[AccountUserProfileProvider.Fields.ShowFamilyNameFirst]);

                    if (showFamilyNameFirst)
                    {
                        return familyName + " " + givenNames;
                    }
                    return givenNames + " " + familyName;
                }
                return givenNames + " " + familyName;
            }
            else if (hasFamilyName)
            {
                return profile[AccountUserProfileProvider.Fields.FamilyName];
            }
            else if (hasGivenNames)
            {
                return profile[AccountUserProfileProvider.Fields.GivenNames];
            }
            else
            {
                return user.UserName;
            }
        }

        #endregion Users

        #region Set<ApplicationRole>()

        public async Task<IEnumerable<MantleRole>> GetAllRoles(int? tenantId)
        {
            IQueryable<ApplicationRole> query = roleManager.Roles;

            if (tenantId.HasValue)
            {
                query = query.Where(x => x.TenantId == tenantId);
            }
            else
            {
                query = query.Where(x => x.TenantId == null);
            }

            return await query
                .Select(x => new MantleRole
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }

        public async Task<MantleRole> GetRoleById(object roleId)
        {
            string id = roleId.ToString();
            var role = await roleManager.FindByIdAsync(id);

            return new MantleRole
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public async Task<IEnumerable<MantleRole>> GetRolesByIds(IEnumerable<object> roleIds)
        {
            var ids = roleIds.ToListOf<string>();
            var roles = new List<ApplicationRole>();

            foreach (string id in ids)
            {
                var role = await roleManager.FindByIdAsync(id);
                roles.Add(role);
            }

            return roles.Select(x => new MantleRole
            {
                Id = x.Id,
                Name = x.Name
            });
        }

        public async Task<MantleRole> GetRoleByName(int? tenantId, string roleName)
        {
            ApplicationRole role;

            if (tenantId.HasValue)
            {
                role = await roleManager.Roles.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Name == roleName);
            }
            else
            {
                role = await roleManager.Roles.FirstOrDefaultAsync(x => x.TenantId == null && x.Name == roleName);
            }

            if (role == null)
            {
                return null;
            }

            return new MantleRole
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public async Task<bool> DeleteRole(object roleId)
        {
            string id = roleId.ToString();
            var role = await roleManager.FindByIdAsync(id);

            if (role != null)
            {
                var result = await roleManager.DeleteAsync(role);
                return result.Succeeded;
            }

            return false;
        }

        public async Task InsertRole(MantleRole role)
        {
            var result = await roleManager.CreateAsync(new ApplicationRole
            {
                TenantId = role.TenantId,
                Name = role.Name
            });

            if (!result.Succeeded)
            {
                string errorMessage = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
                throw new MantleException(errorMessage);
            }
        }

        public async Task UpdateRole(MantleRole role)
        {
            string id = role.Id.ToString();
            var existingRole = await roleManager.Roles.FirstOrDefaultAsync(x => x.Id == id);

            if (existingRole != null)
            {
                existingRole.Name = role.Name;
                var result = await roleManager.UpdateAsync(existingRole);

                if (!result.Succeeded)
                {
                    string errorMessage = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
                    throw new MantleException(errorMessage);
                }
            }
        }

        public async Task<IEnumerable<MantleUser>> GetUsersByRoleId(object roleId)
        {
            string rId = roleId.ToString();

            // TODO: Include(x => x.Users) won't work. It should map to a junction table first (AspNetUserRoles) and then get the Users from that.
            //      userManager.GetUsersInRoleAsync(role.Name) // <-- probably need a custom UserManager (to take TenantId into account) and use this to get the users
            var role = await roleManager.Roles.Include(x => x.Users).FirstOrDefaultAsync(x => x.Id == rId);

            var userIds = role.Users.Select(x => x.Id).ToList();
            var users = await userManager.Users.Where(x => userIds.Contains(x.Id)).ToHashSetAsync();

            return users.Select(x => new MantleUser
            {
                Id = x.Id,
                TenantId = x.TenantId,
                UserName = x.UserName,
                Email = x.Email,
                IsLockedOut = x.LockoutEnabled
            });
        }

        public async Task<IEnumerable<MantleUser>> GetUsersByRoleName(int? tenantId, string roleName)
        {
            ApplicationRole role;

            if (tenantId.HasValue)
            {
                role = await roleManager.Roles
                    .Include(x => x.Users)
                    .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Name == roleName);
            }
            else
            {
                role = await roleManager.Roles
                    .Include(x => x.Users)
                    .FirstOrDefaultAsync(x => x.TenantId == null && x.Name == roleName);
            }

            var userIds = role.Users.Select(x => x.Id).ToList();
            var users = await userManager.Users.Where(x => userIds.Contains(x.Id)).ToHashSetAsync();

            return users.Select(x => new MantleUser
            {
                Id = x.Id,
                TenantId = x.TenantId,
                UserName = x.UserName,
                Email = x.Email,
                IsLockedOut = x.LockoutEnabled
            });
        }

        #endregion Set<ApplicationRole>()

        #region Permissions

        public async Task<IEnumerable<MantlePermission>> GetAllPermissions(int? tenantId)
        {
            using (var context = contextFactory.GetContext())
            {
                IQueryable<Permission> query = context.Set<Permission>();

                if (tenantId.HasValue)
                {
                    query = query.Where(x => x.TenantId == tenantId);
                }
                else
                {
                    query = query.Where(x => x.TenantId == null);
                }

                return (await query.ToListAsync()).Select(x => new MantlePermission
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Category = x.Category,
                    Description = x.Description
                }).ToList();
            }
        }

        public async Task<MantlePermission> GetPermissionById(object permissionId)
        {
            int id = Convert.ToInt32(permissionId);

            using (var context = contextFactory.GetContext())
            {
                var entity = await context.Set<Permission>().FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return null;
                }

                return new MantlePermission
                {
                    Id = entity.Id.ToString(),
                    Name = entity.Name,
                    Category = entity.Category,
                    Description = entity.Description
                };
            }
        }

        public async Task<MantlePermission> GetPermissionByName(int? tenantId, string permissionName)
        {
            Permission entity = null;

            using (var context = contextFactory.GetContext())
            {
                if (tenantId.HasValue)
                {
                    entity = await context.Set<Permission>().FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Name == permissionName);
                }
                else
                {
                    entity = await context.Set<Permission>().FirstOrDefaultAsync(x => x.TenantId == null && x.Name == permissionName);
                }

                if (entity == null)
                {
                    return null;
                }

                return new MantlePermission
                {
                    Id = entity.Id.ToString(),
                    Name = entity.Name,
                    Category = entity.Category,
                    Description = entity.Description
                };
            }
        }

        public async Task<IEnumerable<MantlePermission>> GetPermissionsForRole(int? tenantId, string roleName)
        {
            if (cachedRolePermissions.ContainsKey(roleName))
            {
                return cachedRolePermissions[roleName];
            }

            using (var context = contextFactory.GetContext())
            {
                var query = context.Set<Permission>().Include(x => x.RolesPermissions);

                List<Permission> permissions = null;
                if (tenantId.HasValue)
                {
                    permissions = await (from p in query
                                         from rp in p.RolesPermissions
                                         where p.TenantId == tenantId && rp.Role.Name == roleName
                                         select p).ToListAsync();
                }
                else
                {
                    permissions = await (from p in query
                                         from rp in p.RolesPermissions
                                         where p.TenantId == null && rp.Role.Name == roleName
                                         select p).ToListAsync();
                }

                var rolePermissions = permissions.Select(x => new MantlePermission
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    Category = x.Category,
                    Description = x.Description
                }).ToList();

                cachedRolePermissions.Add(roleName, rolePermissions);
                return rolePermissions;
            }
        }

        public async Task AssignPermissionsToRole(object roleId, IEnumerable<object> permissionIds)
        {
            string rId = roleId.ToString();

            if (roleId is Array)
            {
                rId = ((Array)roleId).GetValue(0).ToString();
            }

            var pIds = permissionIds.ToListOf<int>();
            using (var context = contextFactory.GetContext())
            {
                var set = context.Set<RolePermission>();
                var toDelete = set.Where(x => x.RoleId == rId).ToList();
                set.RemoveRange(toDelete);
                await context.SaveChangesAsync();

                var toInsert = pIds.Select(x => new RolePermission
                {
                    PermissionId = x,
                    RoleId = rId
                });

                set.AddRange(toInsert);
                await context.SaveChangesAsync();

                string roleName = (await GetRoleById(rId)).Name;
                cachedRolePermissions.Remove(roleName);

                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeletePermission(object permissionId)
        {
            var id = Convert.ToInt32(permissionId);

            using (var context = contextFactory.GetContext())
            {
                var existing = await context.Set<Permission>().FirstOrDefaultAsync(x => x.Id == id);

                if (existing != null)
                {
                    context.Set<Permission>().Remove(existing);
                    int rowsAffected = await context.SaveChangesAsync();
                    return rowsAffected > 0;
                }
            }

            return false;
        }

        public async Task<bool> DeletePermissions(IEnumerable<object> permissionIds)
        {
            var ids = permissionIds.Select(x => x.ToString()).ToListOf<int>();

            using (var context = contextFactory.GetContext())
            {
                var toDelete = await context.Set<Permission>().Where(x => ids.Contains(x.Id)).ToListAsync();

                if (toDelete.Any())
                {
                    context.Set<Permission>().RemoveRange(toDelete);
                    int rowsAffected = await context.SaveChangesAsync();
                    return rowsAffected > 0;
                }
            }

            return false;
        }

        public async Task InsertPermission(MantlePermission permission)
        {
            using (var context = contextFactory.GetContext())
            {
                context.Set<Permission>().Add(new Permission
                {
                    TenantId = permission.TenantId,
                    Name = permission.Name,
                    Category = permission.Category,
                    Description = permission.Description
                });
                await context.SaveChangesAsync();
            }
        }

        public async Task InsertPermissions(IEnumerable<MantlePermission> permissions)
        {
            var toInsert = permissions.Select(x => new Permission
            {
                TenantId = x.TenantId,
                Name = x.Name,
                Category = x.Category,
                Description = x.Description
            });

            using (var context = contextFactory.GetContext())
            {
                context.Set<Permission>().AddRange(toInsert);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdatePermission(MantlePermission permission)
        {
            var id = Convert.ToInt32(permission.Id);

            using (var context = contextFactory.GetContext())
            {
                var existing = await context.Set<Permission>().FirstOrDefaultAsync(x => x.Id == id);
                existing.Name = permission.Name;
                existing.Category = permission.Category;
                existing.Description = permission.Description;
                await context.SaveChangesAsync();
            }
        }

        #endregion Permissions

        #region Profile

        public async Task<IDictionary<string, string>> GetProfile(string userId)
        {
            using (var connection = userProfileRepository.OpenConnection())
            {
                return await connection.Query(x => x.UserId == userId).ToDictionaryAsync(k => k.Key, v => v.Value);
            }
        }

        public async Task<IEnumerable<UserProfile>> GetProfiles(IEnumerable<string> userIds)
        {
            using (var connection = userProfileRepository.OpenConnection())
            {
                var entries = await connection.Query(x => userIds.Contains(x.UserId)).ToListAsync();
                return entries.GroupBy(x => x.UserId).Select(x => new UserProfile
                {
                    UserId = x.Key,
                    Profile = x.ToDictionary(k => k.Key, v => v.Value)
                });
            }
        }

        public async Task UpdateProfile(string userId, IDictionary<string, string> profile, bool deleteExisting = false)
        {
            List<UserProfileEntry> entries = null;
            using (var connection = userProfileRepository.OpenConnection())
            {
                entries = await connection.Query(x => x.UserId == userId).ToListAsync();
            }

            if (deleteExisting)
            {
                await userProfileRepository.DeleteAsync(entries);

                var toInsert = profile.Select(x => new UserProfileEntry
                {
                    UserId = userId,
                    Key = x.Key,
                    Value = x.Value
                }).ToList();

                await userProfileRepository.InsertAsync(toInsert);
            }
            else
            {
                var toUpdate = new List<UserProfileEntry>();
                var toInsert = new List<UserProfileEntry>();

                foreach (var keyValue in profile)
                {
                    var existing = entries.FirstOrDefault(x => x.Key == keyValue.Key);

                    if (existing != null)
                    {
                        existing.Value = keyValue.Value;
                        toUpdate.Add(existing);
                    }
                    else
                    {
                        toInsert.Add(new UserProfileEntry
                        {
                            UserId = userId,
                            Key = keyValue.Key,
                            Value = keyValue.Value
                        });
                    }
                }

                if (toUpdate.Any())
                {
                    await userProfileRepository.UpdateAsync(toUpdate);
                }

                if (toInsert.Any())
                {
                    await userProfileRepository.InsertAsync(toInsert);
                }
            }
        }

        public async Task<string> GetProfileEntry(string userId, string key)
        {
            var entry = await userProfileRepository.FindOneAsync(x =>
                x.UserId == userId &&
                x.Key == key);

            if (entry != null)
            {
                return entry.Value;
            }

            return null;
        }

        public async Task SaveProfileEntry(string userId, string key, string value)
        {
            var entry = await userProfileRepository.FindOneAsync(x =>
                x.UserId == userId &&
                x.Key == key);

            if (entry != null)
            {
                entry.Value = value;
                await userProfileRepository.UpdateAsync(entry);
            }
            else
            {
                await userProfileRepository.InsertAsync(new UserProfileEntry
                {
                    UserId = userId,
                    Key = key,
                    Value = value
                });
            }
        }

        public async Task DeleteProfileEntry(string userId, string key)
        {
            var entry = await userProfileRepository.FindOneAsync(x =>
                x.UserId == userId &&
                x.Key == key);

            if (entry != null)
            {
                await userProfileRepository.DeleteAsync(entry);
            }
        }

        public async Task<IEnumerable<MantleUserProfileEntry>> GetProfileEntriesByKey(int? tenantId, string key)
        {
            using (var connection = userProfileRepository.OpenConnection())
            {
                var query = connection.Query();

                if (tenantId.HasValue)
                {
                    query = query.Where(x => x.TenantId == tenantId && x.Key == key);
                }
                else
                {
                    query = query.Where(x => x.TenantId == null && x.Key == key);
                }

                return (await query.ToHashSetAsync())
                    .Select(x => new MantleUserProfileEntry
                    {
                        Id = x.Id.ToString(),
                        UserId = x.UserId,
                        Key = x.Key,
                        Value = x.Value
                    });
            }
        }

        public async Task<IEnumerable<MantleUserProfileEntry>> GetProfileEntriesByKeyAndValue(int? tenantId, string key, string value)
        {
            using (var connection = userProfileRepository.OpenConnection())
            {
                var query = connection.Query();

                if (tenantId.HasValue)
                {
                    query = query.Where(x => x.TenantId == tenantId && x.Key == key && x.Value == value);
                }
                else
                {
                    query = query.Where(x => x.TenantId == null && x.Key == key && x.Value == value);
                }

                return (await query.ToHashSetAsync())
                    .Select(x => new MantleUserProfileEntry
                    {
                        Id = x.Id.ToString(),
                        UserId = x.UserId,
                        Key = x.Key,
                        Value = x.Value
                    });
            }
        }

        public async Task<bool> ProfileEntryExists(int? tenantId, string key, string value, string userId = null)
        {
            using (var connection = userProfileRepository.OpenConnection())
            {
                IQueryable<UserProfileEntry> query = null;

                if (tenantId.HasValue)
                {
                    query = connection.Query(x => x.TenantId == tenantId && x.Key == key && x.Value == value);
                }
                else
                {
                    query = connection.Query(x => x.TenantId == null && x.Key == key && x.Value == value);
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    query = query.Where(x => x.UserId == userId);
                }
                return await query.AnyAsync();
            }
        }

        #endregion Profile

        public async Task EnsureAdminRoleForTenant(int? tenantId)
        {
            if (SupportsRolePermissions)
            {
                var administratorsRole = await GetRoleByName(tenantId, MantleSecurityConstants.Roles.Administrators);
                if (administratorsRole == null)
                {
                    await InsertRole(new MantleRole { TenantId = tenantId, Name = MantleSecurityConstants.Roles.Administrators });
                    administratorsRole = await GetRoleByName(tenantId, MantleSecurityConstants.Roles.Administrators);

                    if (administratorsRole != null)
                    {
                        var permissions = await GetAllPermissions(tenantId);
                        var permissionIds = permissions.Select(x => x.Id);
                        await AssignPermissionsToRole(administratorsRole.Id, permissionIds);

                        // Assign all super admin users (NULL TenantId) to this new admin role
                        var superAdminUsers = await GetUsersByRoleName(null, MantleSecurityConstants.Roles.Administrators);
                        foreach (var user in superAdminUsers)
                        {
                            await AssignUserToRoles(tenantId, user.Id, new[] { administratorsRole.Id });
                        }
                    }
                }
            }
        }

        #endregion IMembershipService Members
    }
}