using Mantle.Localization.Entities;

namespace Mantle.Web.Areas.Admin.Localization.Controllers;

[Authorize]
[Area(MantleWebConstants.Areas.Localization)]
[Route("admin/localization/localizable-strings")]
public class LocalizableStringController : MantleController
{
    private readonly Lazy<ILocalizableStringService> localizableStringService;
    private readonly SiteSettings siteSettings;

    public LocalizableStringController(
        Lazy<ILocalizableStringService> localizableStringService,
        SiteSettings siteSettings)
    {
        this.localizableStringService = localizableStringService;
        this.siteSettings = siteSettings;
    }

    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index() => !CheckPermission(MantleWebPermissions.LocalizableStringsRead) ? Unauthorized() : PartialView();

    [Route("export/{cultureCode}")]
    public IActionResult ExportLanguagePack(string cultureCode)
    {
        int tenantId = WorkContext.CurrentTenant.Id;

        var localizedStrings = localizableStringService.Value.Find(new SearchOptions<LocalizableString>
        {
            Query = x =>
                x.TenantId == tenantId &&
                x.CultureCode == cultureCode &&
                x.TextValue != null
        });

        var languagePack = new LanguagePackFile
        {
            CultureCode = cultureCode,
            LocalizedStrings = localizedStrings.ToDictionary(k => k.TextKey, v => v.TextValue)
        };

        string json = languagePack.JsonSerialize();
        string fileName = string.Format("{0}_LanguagePack_{1}_{2:yyyy-MM-dd}.json", siteSettings.SiteName, cultureCode, DateTime.Now);
        return File(new UTF8Encoding().GetBytes(json), "application/json", fileName);
    }

    [AllowAnonymous]
    [Route("translations.js")]
    public IActionResult GetTranslationsJS()
    {
        var localizedStrings = T.GetAllStrings()
            .ToDictionary(k => k.Name, v => v.Value)
            .Select(x => $"'{x.Key}': '{x.Value.Replace("'", "\\'")}'");

        string js =
$@"class MantleI18N {{
    static dict = {{
        {string.Join($",{Environment.NewLine}\t\t", localizedStrings)}
	}};

    static t(key) {{
        return MantleI18N.dict[key] ?? key;
    }}
}}";

        return File(Encoding.UTF8.GetBytes(js), "text/javascript");
    }
}