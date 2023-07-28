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

    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("get-translations")]
    public JsonResult GetTranslations()
    {
        return Json(new
        {
            Edit = T[MantleWebLocalizableStrings.General.Edit].Value,
            ExecutedTaskSuccess = T[MantleWebLocalizableStrings.ScheduledTasks.ExecutedTaskSuccess].Value,
            ExecutedTaskError = T[MantleWebLocalizableStrings.ScheduledTasks.ExecutedTaskError].Value,
            GetRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
            RunNow = T[MantleWebLocalizableStrings.ScheduledTasks.RunNow].Value,
            UpdateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
            UpdateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
            Columns = new
            {
                Enabled = T[MantleWebLocalizableStrings.ScheduledTasks.Model.Enabled].Value,
                LastEndUtc = T[MantleWebLocalizableStrings.ScheduledTasks.Model.LastEndUtc].Value,
                LastStartUtc = T[MantleWebLocalizableStrings.ScheduledTasks.Model.LastStartUtc].Value,
                LastSuccessUtc = T[MantleWebLocalizableStrings.ScheduledTasks.Model.LastSuccessUtc].Value,
                Name = T[MantleWebLocalizableStrings.ScheduledTasks.Model.Name].Value,
                Seconds = T[MantleWebLocalizableStrings.ScheduledTasks.Model.Seconds].Value,
                StopOnError = T[MantleWebLocalizableStrings.ScheduledTasks.Model.StopOnError].Value,
            }
        });
    }
}