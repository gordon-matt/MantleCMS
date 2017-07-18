using System;
using System.Threading.Tasks;
using Mantle.Caching;
using Mantle.Localization.Services;
using Mantle.Messaging.Domain;
using Mantle.Messaging.Services;
using Mantle.Web.Mvc;
using Mantle.Web.Security.Membership.Permissions;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Messaging.Controllers.Api
{
    [Area("Admin/Messaging")]
    [Route("api/messaging/queued-email")]
    public class QueuedEmailApiController : MantleGenericTenantDataController<QueuedEmail, Guid>
    {
        private readonly Lazy<ICacheManager> cacheManager;
        private readonly Lazy<ILocalizableStringService> localizableStringService;

        public QueuedEmailApiController(
            IQueuedEmailService service,
            Lazy<ILocalizableStringService> localizableStringService,
            Lazy<ICacheManager> cacheManager)
            : base(service)
        {
            this.localizableStringService = localizableStringService;
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
        public override async Task<IActionResult> Put(Guid key, [FromBody]QueuedEmail entity)
        {
            return await base.Put(key, entity);
        }

        [HttpPost]
        [Route("")]
        public override async Task<IActionResult> Post([FromBody]QueuedEmail entity)
        {
            return await base.Post(entity);
        }

        [HttpDelete]
        [Route("{key}")]
        public override async Task<IActionResult> Delete(Guid key)
        {
            return await base.Delete(key);
        }

        protected override Guid GetId(QueuedEmail entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(QueuedEmail entity)
        {
            entity.Id = Guid.NewGuid();
        }

        protected override Permission ReadPermission
        {
            get { return MessagingPermissions.QueuedEmailsRead; }
        }

        protected override Permission WritePermission
        {
            get { return MessagingPermissions.QueuedEmailsWrite; }
        }
    }
}