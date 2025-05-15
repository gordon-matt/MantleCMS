using Mantle.Localization;
using Mantle.Localization.Entities;
using LanguageEntity = Mantle.Localization.Entities.Language;

namespace Mantle.Web.Areas.Admin.Localization.Controllers.Api;

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

    protected override Guid GetId(LanguageEntity entity) => entity.Id;

    protected override void SetNewId(LanguageEntity entity) => entity.Id = Guid.NewGuid();

    [HttpPost]
    public virtual async Task<IActionResult> ResetLocalizableStrings([FromBody] ODataActionParameters parameters)
    {
        if (!CheckPermission(WritePermission))
        {
            return Unauthorized();
        }

        int tenantId = GetTenantId();
        await localizableStringService.Value.DeleteAsync(x => x.TenantId == tenantId);

        var languagePacks = DependoResolver.Instance.ResolveAll<ILanguagePack>();

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

        string cacheKey = string.Format(CacheKeys.LocalizableStringsPatternFormat, GetTenantId());
        cacheManager.Value.RemoveByPattern(cacheKey);

        return Ok();
    }

    protected override Permission ReadPermission => MantleWebPermissions.LanguagesRead;

    protected override Permission WritePermission => MantleWebPermissions.LanguagesWrite;
}