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
                Forums = T[LocalizableStrings.Forums].Value,
                GetRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                InsertRecordError = T[MantleWebLocalizableStrings.General.InsertRecordError].Value,
                InsertRecordSuccess = T[MantleWebLocalizableStrings.General.InsertRecordSuccess].Value,
                UpdateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                UpdateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                Columns = new
                {
                    Name = T[MantleWebLocalizableStrings.General.Name].Value,
                    DisplayOrder = T[MantleWebLocalizableStrings.General.Order].Value,
                    CreatedOnUtc = T[MantleWebLocalizableStrings.General.DateCreatedUtc].Value
                }
            });
        }
    }
}