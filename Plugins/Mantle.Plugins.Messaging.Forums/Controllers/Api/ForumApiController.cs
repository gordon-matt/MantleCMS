using System;
using System.Threading.Tasks;
using KendoGridBinder.ModelBinder.Mvc;
using Mantle.Caching;
using Mantle.Data;
using Mantle.Plugins.Messaging.Forums.Data.Domain;
using Mantle.Security.Membership.Permissions;
using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Plugins.Messaging.Forums.Controllers.Api
{
    [Area(Constants.RouteArea)]
    [Route("api/plugins/messaging/forums/forums")]
    public class ForumApiController : MantleGenericDataController<Forum, int>
    {
        private readonly Lazy<ICacheManager> cacheManager;

        public ForumApiController(
            IRepository<Forum> repository,
            Lazy<ICacheManager> cacheManager)
            : base(repository)
        {
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
        public override async Task<IActionResult> Get(int key)
        {
            return await base.Get(key);
        }

        [HttpPut]
        [Route("{key}")]
        public override async Task<IActionResult> Put(int key, [FromBody]Forum entity)
        {
            // Client does not send all fields, so we get existing and update only the fields that are sent from the client...
            var originalEntity = await Service.FindOneAsync(key);
            originalEntity.Name = entity.Name;
            originalEntity.Description = entity.Description;
            originalEntity.DisplayOrder = entity.DisplayOrder;

            // ... and we set this too.
            originalEntity.UpdatedOnUtc = DateTime.UtcNow;
            return await base.Put(key, originalEntity);
        }

        [HttpPost]
        [Route("")]
        public override async Task<IActionResult> Post([FromBody]Forum entity)
        {
            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.UpdatedOnUtc = DateTime.UtcNow;
            return await base.Post(entity);
        }

        [HttpDelete]
        [Route("{key}")]
        public override async Task<IActionResult> Delete(int key)
        {
            return await base.Delete(key);
        }

        protected override int GetId(Forum entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(Forum entity)
        {
        }

        protected override Permission ReadPermission
        {
            get { return ForumPermissions.ReadForums; }
        }

        protected override Permission WritePermission
        {
            get { return ForumPermissions.WriteForums; }
        }
    }
}