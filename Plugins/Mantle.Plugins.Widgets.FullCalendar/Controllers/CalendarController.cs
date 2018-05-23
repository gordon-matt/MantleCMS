using Mantle.Web;
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
        public ActionResult Index()
        {
            if (!CheckPermission(FullCalendarPermissions.ReadCalendar))
            {
                return Unauthorized();
            }

            WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.Plugins.Title]);
            WorkContext.Breadcrumbs.Add(T[LocalizableStrings.FullCalendar]);

            ViewBag.Title = T[LocalizableStrings.FullCalendar];

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
                    create = T[MantleWebLocalizableStrings.General.Create].Value,
                    delete = T[MantleWebLocalizableStrings.General.Delete].Value,
                    deleteRecordConfirm = T[MantleWebLocalizableStrings.General.ConfirmDeleteRecord].Value,
                    deleteRecordError = T[MantleWebLocalizableStrings.General.DeleteRecordError].Value,
                    deleteRecordSuccess = T[MantleWebLocalizableStrings.General.DeleteRecordSuccess].Value,
                    edit = T[MantleWebLocalizableStrings.General.Edit].Value,
                    events = T[LocalizableStrings.Events].Value,
                    getRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                    insertRecordError = T[MantleWebLocalizableStrings.General.InsertRecordError].Value,
                    insertRecordSuccess = T[MantleWebLocalizableStrings.General.InsertRecordSuccess].Value,
                    updateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                    updateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                    columns = new
                    {
                        calendar = new
                        {
                            name = T[LocalizableStrings.CalendarModel.Name].Value,
                        },
                        @event = new
                        {
                            name = T[LocalizableStrings.CalendarEventModel.Name].Value,
                            startDateTime = T[LocalizableStrings.CalendarEventModel.StartDateTime].Value,
                            endDateTime = T[LocalizableStrings.CalendarEventModel.EndDateTime].Value
                        }
                    }
                }
            });
        }
    }
}