using System.Collections.Generic;

namespace Mantle.Security.Membership.Permissions
{
    public class StandardPermissions : IPermissionProvider
    {
        public static readonly Permission DashboardAccess = new Permission { Name = "DashboardAccess", Category = "System", Description = "Grant access to dashboard" };
        public static readonly Permission FullAccess = new Permission { Name = "FullAccess", Category = "System", Description = "Grant full system access" };

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                DashboardAccess,
                FullAccess
            };
        }
    }
}