using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Mantle.Security.Membership
{
    //public enum UserStatus
    //{
    //    NotRegistered = 0,
    //    Unconfirmed = 1,
    //    Locked = 2,
    //    Active = 3
    //}

    public interface IMembershipService
    {
        bool SupportsRolePermissions { get; }

        Task<string> GenerateEmailConfirmationToken(object userId);

        Task ConfirmEmail(object userId, string token);

        #region Users

        //IQueryable<MantleUser> GetAllUsersAsQueryable(DbContext context, int? tenantId);

        Task<IEnumerable<MantleUser>> GetAllUsers(int? tenantId);

        Task<IEnumerable<MantleUser>> GetUsers(int? tenantId, Expression<Func<MantleUser, bool>> predicate);

        Task<MantleUser> GetUserById(object userId);

        Task<MantleUser> GetUserByEmail(int? tenantId, string email);

        Task<MantleUser> GetUserByName(int? tenantId, string userName);

        Task<IEnumerable<MantleRole>> GetRolesForUser(object userId);

        Task<bool> DeleteUser(object userId);

        Task InsertUser(MantleUser user, string password);

        Task UpdateUser(MantleUser user);

        Task AssignUserToRoles(int? tenantId, object userId, IEnumerable<object> roleIds);

        Task ChangePassword(object userId, string newPassword);

        Task<string> GetUserDisplayName(MantleUser user);

        #endregion Users

        #region Roles

        Task<IEnumerable<MantleRole>> GetAllRoles(int? tenantId);

        Task<MantleRole> GetRoleById(object roleId);

        Task<IEnumerable<MantleRole>> GetRolesByIds(IEnumerable<object> roleIds);

        Task<MantleRole> GetRoleByName(int? tenantId, string roleName);

        Task<bool> DeleteRole(object roleId);

        Task InsertRole(MantleRole role);

        Task UpdateRole(MantleRole role);

        Task<IEnumerable<MantleUser>> GetUsersByRoleId(object roleId);

        Task<IEnumerable<MantleUser>> GetUsersByRoleName(int? tenantId, string roleName);

        #endregion Roles

        #region Permissions

        Task<IEnumerable<MantlePermission>> GetAllPermissions(int? tenantId);

        Task<MantlePermission> GetPermissionById(object permissionId);

        Task<MantlePermission> GetPermissionByName(int? tenantId, string permissionName);

        Task<IEnumerable<MantlePermission>> GetPermissionsForRole(int? tenantId, string roleName);

        Task AssignPermissionsToRole(object roleId, IEnumerable<object> permissionIds);

        Task<bool> DeletePermission(object permissionId);

        Task<bool> DeletePermissions(IEnumerable<object> permissionIds);

        Task InsertPermission(MantlePermission permission);

        Task InsertPermissions(IEnumerable<MantlePermission> permissions);

        Task UpdatePermission(MantlePermission permission);

        #endregion Permissions

        #region Profile

        Task<IDictionary<string, string>> GetProfile(string userId);

        Task<IEnumerable<UserProfile>> GetProfiles(IEnumerable<string> userIds);

        Task UpdateProfile(string userId, IDictionary<string, string> profile, bool deleteExisting = false);

        Task<string> GetProfileEntry(string userId, string key);

        Task SaveProfileEntry(string userId, string key, string value);

        Task DeleteProfileEntry(string userId, string key);

        Task<IEnumerable<MantleUserProfileEntry>> GetProfileEntriesByKey(int? tenantId, string key);

        Task<IEnumerable<MantleUserProfileEntry>> GetProfileEntriesByKeyAndValue(int? tenantId, string key, string value);

        Task<bool> ProfileEntryExists(int? tenantId, string key, string value, string userId = null);

        #endregion Profile

        Task EnsureAdminRoleForTenant(int? tenantId);
    }

    public class UserProfile
    {
        public string UserId { get; set; }

        public IDictionary<string, string> Profile { get; set; }
    }
}