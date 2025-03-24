using Mantle.Web.Areas.Admin.Configuration.Models;
using Mantle.Web.Configuration.Services;

namespace Mantle.Web.Areas.Admin.Configuration.Controllers.Api;

public class ThemeApiController : ODataController
{
    private readonly IMantleAuthorizationService authorizationService;
    private readonly IThemeProvider themeProvider;
    private readonly IWorkContext workContext;
    private readonly SiteSettings siteSettings;
    private readonly Lazy<ISettingService> settingsService;

    public ThemeApiController(
        IMantleAuthorizationService authorizationService,
        IThemeProvider themeProvider,
        IWorkContext workContext,
        SiteSettings siteSettings,
        Lazy<ISettingService> settingsService)
    {
        this.authorizationService = authorizationService;
        this.settingsService = settingsService;
        this.siteSettings = siteSettings;
        this.themeProvider = themeProvider;
        this.workContext = workContext;
    }

    // GET: odata/kore/cms/Plugins
    public virtual IEnumerable<EdmThemeConfiguration> Get(ODataQueryOptions<EdmThemeConfiguration> options)
    {
        if (!CheckPermission(MantleWebPermissions.ThemesRead))
        {
            return Enumerable.Empty<EdmThemeConfiguration>().AsQueryable();
        }

        var themes = themeProvider.GetThemeConfigurations()
            .Select(x => (EdmThemeConfiguration)x)
            .ToList();

        foreach (var theme in themes)
        {
            if (theme.Title == siteSettings.DefaultTheme)
            {
                theme.IsDefaultTheme = true;
            }
        }

        var results = options.ApplyTo(themes.AsQueryable());
        return (results as IQueryable<EdmThemeConfiguration>).ToHashSet();
    }

    [HttpPost]
    public virtual void SetTheme([FromBody] ODataActionParameters parameters)
    {
        // TODO: Change return type to IHttpResult and return UnauthorizedResult
        if (!CheckPermission(MantleWebPermissions.ThemesWrite))
        {
            return;
        }

        string themeName = (string)parameters["themeName"];
        var themeConfig = themeProvider.GetThemeConfiguration(themeName);

        siteSettings.DefaultTheme = themeName;

        siteSettings.DefaultFrontendLayoutPath = !string.IsNullOrEmpty(themeConfig.DefaultLayoutPath) ? themeConfig.DefaultLayoutPath : "~/Views/Shared/_Layout.cshtml";

        settingsService.Value.SaveSettings(siteSettings, workContext.CurrentTenant.Id);
        MantleWebConstants.ResetCache();
    }

    protected bool CheckPermission(Permission permission) => authorizationService.TryCheckAccess(permission, workContext.CurrentUser);
}