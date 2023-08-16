namespace Mantle.Web.Messaging.Controllers;

[Authorize]
[Area(MantleWebMessagingConstants.RouteArea)]
[Route("admin/messaging/queued-email")]
public class QueuedEmailController : MantleController
{
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index()
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
}