using System.Collections.Generic;
using Mantle.Web.Security.Membership.Permissions;

namespace Mantle.Web.Areas.Admin.Plugins
{
    public class PluginsPermissions : IPermissionProvider
    {
        public static readonly Permission ManagePlugins = new Permission { Name = "ManagePlugins", Category = "System", Description = "Manage Plugins" };

        public IEnumerable<Permission> GetPermissions()
        {
            yield return ManagePlugins;
        }
    }
}