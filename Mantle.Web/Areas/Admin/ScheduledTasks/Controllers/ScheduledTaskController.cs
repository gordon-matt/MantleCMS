using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Areas.Admin.ScheduledTasks.Controllers
{
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

        [Route("get-view-data")]
        public JsonResult GetViewData()
        {
            return Json(new
            {
                gridPageSize = SiteSettings.Value.DefaultGridPageSize,
                translations = new
                {
                    edit = T[MantleWebLocalizableStrings.General.Edit].Value,
                    executedTaskSuccess = T[MantleWebLocalizableStrings.ScheduledTasks.ExecutedTaskSuccess].Value,
                    executedTaskError = T[MantleWebLocalizableStrings.ScheduledTasks.ExecutedTaskError].Value,
                    getRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                    runNow = T[MantleWebLocalizableStrings.ScheduledTasks.RunNow].Value,
                    updateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                    updateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                    columns = new
                    {
                        enabled = T[MantleWebLocalizableStrings.ScheduledTasks.Model.Enabled].Value,
                        lastEndUtc = T[MantleWebLocalizableStrings.ScheduledTasks.Model.LastEndUtc].Value,
                        lastStartUtc = T[MantleWebLocalizableStrings.ScheduledTasks.Model.LastStartUtc].Value,
                        lastSuccessUtc = T[MantleWebLocalizableStrings.ScheduledTasks.Model.LastSuccessUtc].Value,
                        name = T[MantleWebLocalizableStrings.ScheduledTasks.Model.Name].Value,
                        seconds = T[MantleWebLocalizableStrings.ScheduledTasks.Model.Seconds].Value,
                        stopOnError = T[MantleWebLocalizableStrings.ScheduledTasks.Model.StopOnError].Value,
                    }
                }
            });
        }
    }
}