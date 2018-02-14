using System.Collections.Generic;
using Mantle.Web.Security.Membership.Permissions;

namespace Mantle.Plugins.Widgets.FullCalendar
{
    public class FullCalendarPermissions : IPermissionProvider
    {
        public static readonly Permission ReadCalendar = new Permission { Name = "Plugin_FullCalendar_ReadCalendar", Category = "Plugin - Full Calendar", Description = "Plugin: Full Calendar - Read Calendar" };
        public static readonly Permission WriteCalendar = new Permission { Name = "Plugin_FullCalendar_WriteCalendar", Category = "Plugin - Full Calendar", Description = "Plugin: Full Calendar - Write Calendar" };

        #region IPermissionProvider Members

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ReadCalendar,
                WriteCalendar
            };
        }

        #endregion IPermissionProvider Members
    }
}