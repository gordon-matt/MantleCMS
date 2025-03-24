using Mantle.Plugins.Widgets.FullCalendar.Data.Entities;
using Mantle.Plugins.Widgets.FullCalendar.Services;
using Mantle.Security.Membership.Permissions;
using Mantle.Web.OData;

namespace Mantle.Plugins.Widgets.FullCalendar.Controllers.Api;

public class CalendarApiController : GenericTenantODataController<Calendar, int>
{
    public CalendarApiController(ICalendarService service)
        : base(service)
    {
    }

    protected override int GetId(Calendar entity) => entity.Id;

    protected override void SetNewId(Calendar entity)
    {
    }

    protected override Permission ReadPermission => FullCalendarPermissions.ReadCalendar;

    protected override Permission WritePermission => FullCalendarPermissions.WriteCalendar;
}