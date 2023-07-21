using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Messaging.Controllers
{
    [Authorize]
    [Area(MantleWebMessagingConstants.RouteArea)]
    [Route("admin/messaging/queued-email")]
    public class QueuedEmailController : MantleController
    {
        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("")]
        public ActionResult Index()
        {
            if (!CheckPermission(MessagingPermissions.QueuedEmailsRead))
            {
                return Unauthorized();
            }

            WorkContext.Breadcrumbs.Add(T[LocalizableStrings.Messaging].Value);
            WorkContext.Breadcrumbs.Add(T[LocalizableStrings.QueuedEmails].Value);

            WorkContext.Breadcrumbs.Add(T[LocalizableStrings.Messaging].Value);
            ViewBag.SubTitle = T[LocalizableStrings.QueuedEmails].Value;

            return PartialView("Mantle.Web.Messaging.Views.QueuedEmail.Index");
        }

        [Route("get-view-data")]
        public JsonResult GetViewData()
        {
            return Json(new
            {
                gridPageSize = SiteSettings.Value.DefaultGridPageSize,
                translations = new
                {
                    delete = T[MantleWebLocalizableStrings.General.Delete].Value,
                    deleteRecordConfirm = T[MantleWebLocalizableStrings.General.ConfirmDeleteRecord].Value,
                    deleteRecordError = T[MantleWebLocalizableStrings.General.DeleteRecordError].Value,
                    deleteRecordSuccess = T[MantleWebLocalizableStrings.General.DeleteRecordSuccess].Value,
                    columns = new
                    {
                        createdOnUtc = T[LocalizableStrings.QueuedEmail.CreatedOnUtc].Value,
                        sentOnUtc = T[LocalizableStrings.QueuedEmail.SentOnUtc].Value,
                        sentTries = T[LocalizableStrings.QueuedEmail.SentTries].Value,
                        subject = T[LocalizableStrings.QueuedEmail.Subject].Value,
                        toAddress = T[LocalizableStrings.QueuedEmail.ToAddress].Value
                    }
                }
            });
        }
    }
}