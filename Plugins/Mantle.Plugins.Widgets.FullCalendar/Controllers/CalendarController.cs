namespace Mantle.Plugins.Widgets.FullCalendar.Controllers;

[Authorize]
[Area(Constants.RouteArea)]
[Route("plugins/widgets/full-calendar")]
public class CalendarController : MantleController
{
    //[Compress]
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index() => !CheckPermission(FullCalendarPermissions.ReadCalendar) ? Unauthorized() : PartialView();
}