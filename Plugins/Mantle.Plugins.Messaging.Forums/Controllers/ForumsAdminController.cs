using Mantle.Web;
using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Plugins.Messaging.Forums.Controllers
{
    [Authorize]
    [Route(Constants.RouteArea)]
    public class ForumsAdminController : MantleController
    {
        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("")]
        public IActionResult Index()
        {
            if (!CheckPermission(ForumPermissions.ReadForums))
            {
                return Unauthorized();
            }

            WorkContext.Breadcrumbs.Add(T[LocalizableStrings.Forums], Url.Action("Index"));

            ViewBag.Title = T[LocalizableStrings.Forums];

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
                    forums = T[LocalizableStrings.Forums].Value,
                    getRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                    insertRecordError = T[MantleWebLocalizableStrings.General.InsertRecordError].Value,
                    insertRecordSuccess = T[MantleWebLocalizableStrings.General.InsertRecordSuccess].Value,
                    updateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                    updateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                    columns = new
                    {
                        name = T[MantleWebLocalizableStrings.General.Name].Value,
                        displayOrder = T[MantleWebLocalizableStrings.General.Order].Value,
                        createdOnUtc = T[MantleWebLocalizableStrings.General.DateCreatedUtc].Value
                    }
                }
            });
        }
    }
}