using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Plugins.Widgets.FullCalendar.Controllers
{
    [Authorize]
    [Area(Constants.RouteArea)]
    [Route("plugins/widgets/full-calendar")]
    public class CalendarController : MantleController
    {
        //[Compress]
        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("")]
        public IActionResult Index()
        {
            if (!CheckPermission(FullCalendarPermissions.ReadCalendar))
            {
                return Unauthorized();
            }

            return PartialView();
        }
    }
}