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
        public IActionResult Index()
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

        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("get-translations")]
        public JsonResult GetTranslations()
        {
            return Json(new
            {
                Create = T[MantleWebLocalizableStrings.General.Create].Value,
                Delete = T[MantleWebLocalizableStrings.General.Delete].Value,
                DeleteRecordConfirm = T[MantleWebLocalizableStrings.General.ConfirmDeleteRecord].Value,
                DeleteRecordError = T[MantleWebLocalizableStrings.General.DeleteRecordError].Value,
                DeleteRecordSuccess = T[MantleWebLocalizableStrings.General.DeleteRecordSuccess].Value,
                Edit = T[MantleWebLocalizableStrings.General.Edit].Value,
                Events = T[LocalizableStrings.Events].Value,
                GetRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                InsertRecordError = T[MantleWebLocalizableStrings.General.InsertRecordError].Value,
                InsertRecordSuccess = T[MantleWebLocalizableStrings.General.InsertRecordSuccess].Value,
                UpdateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                UpdateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                Columns = new
                {
                    Calendar = new
                    {
                        Name = T[LocalizableStrings.CalendarModel.Name].Value,
                    },
                    Event = new
                    {
                        Name = T[LocalizableStrings.CalendarEventModel.Name].Value,
                        StartDateTime = T[LocalizableStrings.CalendarEventModel.StartDateTime].Value,
                        EndDateTime = T[LocalizableStrings.CalendarEventModel.EndDateTime].Value
                    }
                }
            });
        }
    }
}