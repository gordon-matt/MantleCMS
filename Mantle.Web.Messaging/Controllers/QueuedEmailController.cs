namespace Mantle.Web.Messaging.Controllers;

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

        return PartialView("/Views/QueuedEmail/Index.cshtml");
    }

    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("get-translations")]
    public JsonResult GetTranslations()
    {
        return Json(new
        {
            Delete = T[MantleWebLocalizableStrings.General.Delete].Value,
            DeleteRecordConfirm = T[MantleWebLocalizableStrings.General.ConfirmDeleteRecord].Value,
            DeleteRecordError = T[MantleWebLocalizableStrings.General.DeleteRecordError].Value,
            DeleteRecordSuccess = T[MantleWebLocalizableStrings.General.DeleteRecordSuccess].Value,
            Columns = new
            {
                CreatedOnUtc = T[LocalizableStrings.QueuedEmail.CreatedOnUtc].Value,
                SentOnUtc = T[LocalizableStrings.QueuedEmail.SentOnUtc].Value,
                SentTries = T[LocalizableStrings.QueuedEmail.SentTries].Value,
                Subject = T[LocalizableStrings.QueuedEmail.Subject].Value,
                ToAddress = T[LocalizableStrings.QueuedEmail.ToAddress].Value
            }
        });
    }
}