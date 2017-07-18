using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mantle.Caching;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Localization.Domain;
using Mantle.Localization.Services;
using Mantle.Web.Mvc;
using Mantle.Web.Mvc.KendoUI;
using Mantle.Web.Security.Membership.Permissions;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using LanguageEntity = Mantle.Localization.Domain.Language;

namespace Mantle.Web.Areas.Admin.Localization.Controllers
{
    [Area(MantleWebConstants.Areas.Localization)]
    [Route("api/localization/languages")]
    public class LanguageApiController : MantleGenericTenantDataController<LanguageEntity, Guid>
    {
        private readonly Lazy<ICacheManager> cacheManager;
        private readonly Lazy<ILocalizableStringService> localizableStringService;

        public LanguageApiController(
            ILanguageService service,
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
        public override async Task<IActionResult> Put(Guid key, [FromBody]LanguageEntity entity)
        {
            return await base.Put(key, entity);
        }

        [HttpPost]
        [Route("")]
        public override async Task<IActionResult> Post([FromBody]LanguageEntity entity)
        {
            return await base.Post(entity);
        }

        [HttpDelete]
        [Route("{key}")]
        public override async Task<IActionResult> Delete(Guid key)
        {
            return await base.Delete(key);
        }

        [HttpPost]
        [Route("Default.ResetLocalizableStrings")]
        public virtual async Task<IActionResult> ResetLocalizableStrings([FromBody]dynamic data)
        {
            if (!CheckPermission(WritePermission))
            {
                return Unauthorized();
            }

            int tenantId = GetTenantId();
            await localizableStringService.Value.DeleteAsync(x => x.TenantId == tenantId);

            var languagePacks = EngineContext.Current.ResolveAll<ILanguagePack>();

            var toInsert = new HashSet<LocalizableString>();
            foreach (var languagePack in languagePacks)
            {
                foreach (var localizedString in languagePack.LocalizedStrings)
                {
                    toInsert.Add(new LocalizableString
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        CultureCode = languagePack.CultureCode,
                        TextKey = localizedString.Key,
                        TextValue = localizedString.Value
                    });
                }
            }
            await localizableStringService.Value.InsertAsync(toInsert);

            cacheManager.Value.RemoveByPattern(MantleConstants.CacheKeys.LocalizableStringsPatternFormat);
            
            return Json(new { Success = true, Message = T[MantleWebLocalizableStrings.Localization.ResetLocalizableStringsSuccess].Value });
        }

        protected override Guid GetId(LanguageEntity entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(LanguageEntity entity)
        {
            entity.Id = Guid.NewGuid();
        }

        protected override Permission ReadPermission
        {
            get { return MantleWebPermissions.LanguagesRead; }
        }

        protected override Permission WritePermission
        {
            get { return MantleWebPermissions.LanguagesWrite; }
        }
    }
}