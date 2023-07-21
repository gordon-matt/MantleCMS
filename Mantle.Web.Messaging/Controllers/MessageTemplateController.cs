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

        [Route("get-view-data")]
        public JsonResult GetViewData()
        {
            return Json(new
            {
                availableEditors = messageTemplateEditors,
                gridPageSize = SiteSettings.Value.DefaultGridPageSize,
                translations = new
                {
                    create = T[MantleWebLocalizableStrings.General.Create].Value,
                    delete = T[MantleWebLocalizableStrings.General.Delete].Value,
                    deleteRecordConfirm = T[MantleWebLocalizableStrings.General.ConfirmDeleteRecord].Value,
                    deleteRecordError = T[MantleWebLocalizableStrings.General.DeleteRecordError].Value,
                    deleteRecordSuccess = T[MantleWebLocalizableStrings.General.DeleteRecordSuccess].Value,
                    edit = T[MantleWebLocalizableStrings.General.Edit].Value,
                    editWithGrapesJS = string.Format(T[MantleWebLocalizableStrings.General.EditWithFormat].Value, "GrapesJS"),
                    getRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                    getTokensError = T[LocalizableStrings.GetTokensError].Value,
                    insertRecordError = T[MantleWebLocalizableStrings.General.InsertRecordError].Value,
                    insertRecordSuccess = T[MantleWebLocalizableStrings.General.InsertRecordSuccess].Value,
                    toggle = T[MantleWebLocalizableStrings.General.Toggle].Value,
                    updateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                    updateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                    columns = new
                    {
                        name = T[LocalizableStrings.MessageTemplate.Name].Value,
                        editor = T[LocalizableStrings.MessageTemplate.Editor].Value,
                        enabled = T[MantleWebLocalizableStrings.General.Enabled].Value
                    }
                }
            });
        }
    }
}