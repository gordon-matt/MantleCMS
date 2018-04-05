using Mantle.Messaging.Data.Domain;
using Mantle.Messaging.Services;
using Mantle.Web.Messaging.Models;
using Mantle.Web.OData;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Messaging.Controllers.Api
{
    public class MessageTemplateVersionApiController : GenericODataController<MessageTemplateVersion, int>
    {
        public MessageTemplateVersionApiController(
            IMessageTemplateVersionService service)
            : base(service)
        {
        }

        protected override int GetId(MessageTemplateVersion entity)
        {
            return entity.Id;
        }

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
                return NotFound();
            }

            if (currentVersion.Data.Contains("gjs-assets"))
            {
                if (currentVersion.Data.Contains("mjml"))
                {
                    return BadRequest("Please open this template in GrapesJS.");
                }

                // Since we wish to edit in normal HTML editor this time (was GrapesJS before), then we need to extract just the HTML
                var data = currentVersion.Data.JsonDeserialize<GrapesJsStorageData>();
                currentVersion.Data = data.Html;
            }

            return Ok(currentVersion);
        }

        protected override Permission ReadPermission
        {
            get { return MessagingPermissions.MessageTemplatesRead; }
        }

        protected override Permission WritePermission
        {
            get { return MessagingPermissions.MessageTemplatesWrite; }
        }
    }
}