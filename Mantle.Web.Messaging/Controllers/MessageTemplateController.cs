using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Messaging.Controllers
{
    [Authorize]
    [Area(MantleWebMessagingConstants.RouteArea)]
    [Route("admin/messaging/templates")]
    public class MessageTemplateController : MantleController
    {
        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("")]
        public ActionResult Index()
        {
            if (!CheckPermission(MessagingPermissions.MessageTemplatesRead))
            {
                return Unauthorized();
            }

            WorkContext.Breadcrumbs.Add(T[LocalizableStrings.Messaging].Value);
            WorkContext.Breadcrumbs.Add(T[LocalizableStrings.MessageTemplates].Value);

            ViewBag.Title = T[LocalizableStrings.Messaging].Value;
            ViewBag.SubTitle = T[LocalizableStrings.MessageTemplates].Value;

            return PartialView("Mantle.Web.Messaging.Views.MessageTemplate.Index");
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
                GetRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                GetTokensError = T[LocalizableStrings.GetTokensError].Value,
                InsertRecordError = T[MantleWebLocalizableStrings.General.InsertRecordError].Value,
                InsertRecordSuccess = T[MantleWebLocalizableStrings.General.InsertRecordSuccess].Value,
                Toggle = T[MantleWebLocalizableStrings.General.Toggle].Value,
                UpdateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                UpdateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                Columns = new
                {
                    Name = T[LocalizableStrings.MessageTemplate.Name].Value,
                    Subject = T[LocalizableStrings.MessageTemplate.Subject].Value,
                    Enabled = T[MantleWebLocalizableStrings.General.Enabled].Value
                }
            });
        }
    }
}