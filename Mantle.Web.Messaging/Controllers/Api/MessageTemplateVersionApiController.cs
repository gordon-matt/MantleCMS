﻿using Mantle.Messaging.Data.Entities;
using Mantle.Messaging.Services;
using Microsoft.AspNetCore.OData.Formatter;

namespace Mantle.Web.Messaging.Controllers.Api;

public class MessageTemplateVersionApiController : GenericODataController<MessageTemplateVersion, int>
{
    private readonly IMessageTemplateService messageTemplateService;

    public MessageTemplateVersionApiController(
        IMessageTemplateVersionService service,
        IMessageTemplateService messageTemplateService)
        : base(service)
    {
        this.messageTemplateService = messageTemplateService;
    }

    protected override int GetId(MessageTemplateVersion entity) => entity.Id;

    protected override void SetNewId(MessageTemplateVersion entity)
    {
    }

    [HttpGet]
    public IActionResult GetCurrentVersion([FromODataUri] int templateId, [FromODataUri] string cultureCode)
    {
        if (!CheckPermission(WritePermission))
        {
            return Unauthorized();
        }

        var currentVersion = ((IMessageTemplateVersionService)Service).FindOne(
            templateId,
            cultureCode);

        if (currentVersion == null)
        {
            var template = messageTemplateService.FindOne(templateId);

            var utcNow = DateTime.UtcNow;
            currentVersion = new MessageTemplateVersion
            {
                MessageTemplateId = templateId,
                CultureCode = cultureCode,
                Subject = template.Name,
                DateCreatedUtc = utcNow,
                DateModifiedUtc = utcNow
            };
            Service.Insert(currentVersion);
        }
        //else if (!string.IsNullOrEmpty(currentVersion.Data) && currentVersion.Data.Contains("gjs-assets"))
        //{
        //    if (currentVersion.Data.Contains("mjml"))
        //    {
        //        return BadRequest("Please open this template in GrapesJS.");
        //    }
        //    // Since we wish to edit in normal HTML editor this time (was GrapesJS before), then we need to extract just the HTML
        //    var data = currentVersion.Data.JsonDeserialize<GrapesJsStorageData>();
        //    currentVersion.Data = data.Html;
        //}

        //return new JsonResult(JObject.FromObject(currentVersion));
        return Ok(currentVersion);
    }

    protected override Permission ReadPermission => MessagingPermissions.MessageTemplatesRead;

    protected override Permission WritePermission => MessagingPermissions.MessageTemplatesWrite;
}