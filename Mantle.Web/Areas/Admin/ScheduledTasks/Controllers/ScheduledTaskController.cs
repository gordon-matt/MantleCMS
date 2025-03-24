namespace Mantle.Web.Areas.Admin.ScheduledTasks.Controllers;

[Authorize]
[Area(MantleWebConstants.Areas.ScheduledTasks)]
[Route("admin/scheduled-tasks")]
public class ScheduledTaskController : MantleController
{
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index() => !CheckPermission(MantleWebPermissions.ScheduledTasksRead)
        ? Unauthorized()
        : PartialView();
}