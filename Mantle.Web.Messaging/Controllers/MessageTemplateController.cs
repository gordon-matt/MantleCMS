﻿namespace Mantle.Web.Messaging.Controllers;

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
    public IActionResult Index() => !CheckPermission(MessagingPermissions.MessageTemplatesRead)
        ? Unauthorized()
        : PartialView("/Views/MessageTemplate/Index.cshtml");

    [Route("get-available-editors")]
    public JsonResult GetAvailableEditors() => Json(messageTemplateEditors);
}