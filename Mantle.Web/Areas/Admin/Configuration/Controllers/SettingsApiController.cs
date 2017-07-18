using System;
using System.Threading.Tasks;
using Mantle.Caching;
using Mantle.Configuration.Domain;
using Mantle.Data;
using Mantle.Web.Configuration;
using Mantle.Web.Mvc;
using Mantle.Web.Mvc.KendoUI;
using Mantle.Web.Security.Membership.Permissions;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Areas.Admin.Configuration.Controllers.Api
{
    [Area(MantleWebConstants.Areas.Configuration)]
    [Route("api/configuration/settings")]
    public class SettingsApiController : MantleGenericTenantDataController<Setting, Guid>
    {
        private readonly ICacheManager cacheManager;

        public SettingsApiController(IRepository<Setting> repository, ICacheManager cacheManager)
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
        public override async Task<IActionResult> Get(Guid key)
        {
            return await base.Get(key);
        }

        [HttpPut]
        [Route("{key}")]
        public override async Task<IActionResult> Put(Guid key, [FromBody]Setting entity)
        {
            var result = await base.Put(key, entity);

            string cacheKey = string.Format(MantleWebConstants.CacheKeys.SettingsKeyFormat, entity.TenantId, entity.Type);
            cacheManager.Remove(cacheKey);

            // TODO: This is an ugly hack. We need to have a way for each setting to perform some tasks after update
            if (entity.Name == new SiteSettings().Name)
            {
                cacheManager.Remove(MantleWebConstants.CacheKeys.CurrentCulture);
            }

            return result;
        }

        [HttpPost]
        [Route("")]
        public override async Task<IActionResult> Post([FromBody]Setting entity)
        {
            var result = await base.Post(entity);

            string cacheKey = string.Format(MantleWebConstants.CacheKeys.SettingsKeyFormat, entity.TenantId, entity.Type);
            cacheManager.Remove(cacheKey);

            return result;
        }

        [HttpDelete]
        [Route("{key}")]
        public override async Task<IActionResult> Delete(Guid key)
        {
            var result = base.Delete(key);

            var entity = await Service.FindOneAsync(key);
            string cacheKey = string.Format(MantleWebConstants.CacheKeys.SettingsKeyFormat, entity.TenantId, entity.Type);
            cacheManager.Remove(cacheKey);

            return await result;
        }

        protected override Guid GetId(Setting entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(Setting entity)
        {
            entity.Id = Guid.NewGuid();
        }

        protected override Permission ReadPermission
        {
            get { return MantleWebPermissions.SettingsRead; }
        }

        protected override Permission WritePermission
        {
            get { return MantleWebPermissions.SettingsWrite; }
        }
    }
}