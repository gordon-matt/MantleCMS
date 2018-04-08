using System.Collections.Generic;
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
        private readonly IEnumerable<IMessageTemplateEditor> messageTemplateEditors;

        public MessageTemplateController(IEnumerable<IMessageTemplateEditor> messageTemplateEditors)
        {
            this.messageTemplateEditors = messageTemplateEditors;
        }

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
                EditWithGrapesJS = string.Format(T[MantleWebLocalizableStrings.General.EditWithFormat].Value, "GrapesJS"),
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
                    Editor = T[LocalizableStrings.MessageTemplate.Editor].Value,
                    Enabled = T[MantleWebLocalizableStrings.General.Enabled].Value
                }
            });
        }

        [Route("get-available-editors")]
        public JsonResult GetAvailableEditors()
        {
            return Json(messageTemplateEditors);
        }
    }
}