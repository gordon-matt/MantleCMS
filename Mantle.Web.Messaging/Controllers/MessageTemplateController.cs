namespace Mantle.Web.Messaging.Controllers;

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
    public IActionResult Index()
    {
        if (!CheckPermission(MessagingPermissions.MessageTemplatesRead))
        {
            return Unauthorized();
        }

        WorkContext.Breadcrumbs.Add(T[LocalizableStrings.Messaging].Value);
        WorkContext.Breadcrumbs.Add(T[LocalizableStrings.MessageTemplates].Value);

        ViewBag.Title = T[LocalizableStrings.Messaging].Value;
        ViewBag.SubTitle = T[LocalizableStrings.MessageTemplates].Value;

        return PartialView("/Views/MessageTemplate/Index.cshtml");
    }

    [Route("get-available-editors")]
    public JsonResult GetAvailableEditors()
    {
        return Json(messageTemplateEditors);
    }
}