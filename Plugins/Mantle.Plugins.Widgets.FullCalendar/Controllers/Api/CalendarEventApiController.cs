using Mantle.Plugins.Widgets.FullCalendar.Data.Entities;
using Mantle.Plugins.Widgets.FullCalendar.Services;
using Mantle.Security.Membership.Permissions;
using Mantle.Web.OData;

namespace Mantle.Plugins.Widgets.FullCalendar.Controllers.Api;

public class CalendarEventApiController : GenericODataController<CalendarEvent, int>
{
    public CalendarEventApiController(ICalendarEventService service)
        : base(service)
    {
    }

    protected override int GetId(CalendarEvent entity) => entity.Id;

    protected override void SetNewId(CalendarEvent entity)
    {
    }

    protected override Permission ReadPermission => FullCalendarPermissions.ReadCalendar;

    protected override Permission WritePermission => FullCalendarPermissions.WriteCalendar;
}