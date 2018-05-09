using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mantle.Caching;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Localization.Domain;
using Mantle.Localization.Services;
using Mantle.Security.Membership.Permissions;
using Mantle.Web.OData;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using LanguageEntity = Mantle.Localization.Domain.Language;

namespace Mantle.Web.Areas.Admin.Localization.Controllers.Api
{
    public class LanguageApiController : GenericTenantODataController<LanguageEntity, Guid>
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

        protected override Guid GetId(LanguageEntity entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(LanguageEntity entity)
        {
            entity.Id = Guid.NewGuid();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ResetLocalizableStrings([FromBody] ODataActionParameters parameters)
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

            cacheManager.Value.RemoveByPattern(CacheKeys.LocalizableStringsPatternFormat);

            return Ok();
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