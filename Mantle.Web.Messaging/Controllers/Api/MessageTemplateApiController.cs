using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Mantle.Caching;
using Mantle.Localization.Services;
using Mantle.Messaging.Domain;
using Mantle.Messaging.Services;
using Mantle.Web.Mvc;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Messaging.Controllers.Api
{
    [Area("Admin/Messaging")]
    [Route("api/messaging/templates")]
    public class MessageTemplateApiController : MantleGenericTenantDataController<MessageTemplate, Guid>
    {
        private readonly Lazy<ICacheManager> cacheManager;
        private readonly Lazy<ILocalizableStringService> localizableStringService;
        private readonly Lazy<IEnumerable<IMessageTemplateTokensProvider>> tokensProviders;

        public MessageTemplateApiController(
            IMessageTemplateService service,
            Lazy<ILocalizableStringService> localizableStringService,
            Lazy<IEnumerable<IMessageTemplateTokensProvider>> tokensProviders,
            Lazy<ICacheManager> cacheManager)
            : base(service)
        {
            this.localizableStringService = localizableStringService;
            this.tokensProviders = tokensProviders;
            this.cacheManager = cacheManager;
        }

        [HttpPost]
        [Route("get")]
        public override async Task<IActionResult> Get([FromBody]KendoGridMvcRequest request)
        {
            return await base.Get(request);
        }

        [HttpGet]
        [Route("{key}")]
        public override async Task<IActionResult> Get(Guid key)
        {
            return await base.Get(key);
        }

        [HttpPut]
        [Route("{key}")]
        public override async Task<IActionResult> Put(Guid key, [FromBody]MessageTemplate entity)
        {
            return await base.Put(key, entity);
        }

        [HttpPost]
        [Route("")]
        public override async Task<IActionResult> Post([FromBody]MessageTemplate entity)
        {
            return await base.Post(entity);
        }

        [HttpDelete]
        [Route("{key}")]
        public override async Task<IActionResult> Delete(Guid key)
        {
            return await base.Delete(key);
        }

        protected override Guid GetId(MessageTemplate entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(MessageTemplate entity)
        {
            entity.Id = Guid.NewGuid();
        }

        [HttpPost]
        [Route("Default.GetTokens")]
        public virtual IActionResult GetTokens([FromBody]dynamic data)
        {
            if (!CheckPermission(ReadPermission))
            {
                return Unauthorized();
            }

            string templateName = data.templateName;

            var tokens = tokensProviders.Value
                .SelectMany(x => x.GetTokens(templateName))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return Ok(tokens);
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