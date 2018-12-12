using System;
using System.Threading.Tasks;
using Extenso.Data.Entity;
using Mantle.Caching;
using Mantle.Plugins.Messaging.Forums.Data.Domain;
using Mantle.Security.Membership.Permissions;
using Mantle.Web.OData;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Plugins.Messaging.Forums.Controllers.Api
{
    public class ForumApiController : GenericODataController<Forum, int>
    {
        private readonly Lazy<ICacheManager> cacheManager;

        public ForumApiController(
            IRepository<Forum> repository,
            Lazy<ICacheManager> cacheManager)
            : base(repository)
        {
            this.cacheManager = cacheManager;
        }

        protected override int GetId(Forum entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(Forum entity)
        {
        }

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

        public override async Task<IActionResult> Post([FromBody]Forum entity)
        {
            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.UpdatedOnUtc = DateTime.UtcNow;
            return await base.Post(entity);
        }

        protected override Permission ReadPermission => ForumPermissions.ReadForums;

        protected override Permission WritePermission => ForumPermissions.WriteForums;
    }
}