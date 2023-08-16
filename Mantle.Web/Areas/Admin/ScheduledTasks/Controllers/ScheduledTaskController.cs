namespace Mantle.Web.Areas.Admin.ScheduledTasks.Controllers;

[Authorize]
[Area(MantleWebConstants.Areas.ScheduledTasks)]
[Route("admin/scheduled-tasks")]
public class ScheduledTaskController : MantleController
{
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index()
    {
        if (!CheckPermission(MantleWebPermissions.ScheduledTasksRead))
        {
            return Unauthorized();
        }

        WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.ScheduledTasks.Title].Value);

        ViewBag.Title = T[MantleWebLocalizableStrings.ScheduledTasks.Title].Value;
        ViewBag.SubTitle = T[MantleWebLocalizableStrings.ScheduledTasks.ManageScheduledTasks].Value;

        return PartialView();
    }
}