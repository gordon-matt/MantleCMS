using Mantle.Security.Membership.Permissions;

namespace Mantle.Plugins.Widgets.FullCalendar
{
    public class FullCalendarPermissions : IPermissionProvider
    {
        public static readonly Permission ReadCalendar = new() { Name = "Plugin_FullCalendar_ReadCalendar", Category = "Plugin - Full Calendar", Description = "Plugin: Full Calendar - Read Calendar" };
        public static readonly Permission WriteCalendar = new() { Name = "Plugin_FullCalendar_WriteCalendar", Category = "Plugin - Full Calendar", Description = "Plugin: Full Calendar - Write Calendar" };

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