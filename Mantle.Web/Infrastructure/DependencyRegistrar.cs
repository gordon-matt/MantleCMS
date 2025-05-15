using Extenso.AspNetCore.OData;
using Mantle.Localization;
using Mantle.Web.Areas.Admin;
using Mantle.Web.Configuration.Services;
using Mantle.Web.Helpers;
using Mantle.Web.Localization;
using Mantle.Web.Localization.Services;
using Mantle.Web.Mvc.EmbeddedResources;
using Mantle.Web.Mvc.Resources;
using Mantle.Web.Mvc.Routing;
using Mantle.Web.Navigation;
using Mantle.Web.Security.Membership;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.Infrastructure;

public abstract class BaseMantleWebDependencyRegistrar : IDependencyRegistrar
{
    public virtual void Register(IContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        var settings = DataSettingsManager.LoadSettings();
        builder.RegisterInstance(settings);

        // Helpers
        builder.Register<IWebHelper, WebHelper>(ServiceLifetime.Scoped);
        builder.Register<IDateTimeHelper, DateTimeHelper>(ServiceLifetime.Scoped);

        // Plugins
        builder.Register<IPluginFinder, PluginFinder>(ServiceLifetime.Scoped);

        // Work Context, Themes, Routing, etc
        builder.Register<IWorkContext, WorkContext>(ServiceLifetime.Scoped);
        builder.Register<IThemeProvider, ThemeProvider>(ServiceLifetime.Scoped);
        builder.Register<IThemeContext, ThemeContext>(ServiceLifetime.Scoped);
        builder.Register<IEmbeddedResourceResolver, EmbeddedResourceResolver>(ServiceLifetime.Singleton);
        builder.Register<IRoutePublisher, RoutePublisher>(ServiceLifetime.Singleton);

        // Security
        builder.Register<IMantleAuthorizationService, RolesBasedAuthorizationService>(ServiceLifetime.Singleton);

        builder.Register<ISettingService, DefaultSettingService>(ServiceLifetime.Transient);
        builder.Register<ISettings, DateTimeSettings>(ServiceLifetime.Scoped);
        builder.Register<ISettings, SiteSettings>(ServiceLifetime.Scoped);
        builder.Register<ISettings, MembershipSettings>(ServiceLifetime.Scoped);

        // Navigation
        builder.Register<IDurandalRouteProvider, DurandalRouteProvider>(ServiceLifetime.Singleton);
        builder.Register<IRequireJSConfigProvider, RequireJSConfigProvider>(ServiceLifetime.Singleton);
        builder.Register<INavigationManager, NavigationManager>(ServiceLifetime.Transient);
        builder.Register<INavigationProvider, NavigationProvider>(ServiceLifetime.Singleton);

        // Permission Providers
        builder.Register<IPermissionProvider, StandardPermissions>(ServiceLifetime.Singleton);
        builder.Register<IPermissionProvider, MantleWebPermissions>(ServiceLifetime.Singleton);

        // Work Context State Providers
        builder.Register<IWorkContextStateProvider, CurrentUserStateProvider>(ServiceLifetime.Transient);
        builder.Register<IWorkContextStateProvider, CurrentThemeStateProvider>(ServiceLifetime.Transient);
        builder.Register<IWorkContextStateProvider, CurrentCultureCodeStateProvider>(ServiceLifetime.Transient);

        // Localization
        builder.Register<ILanguagePack, LanguagePackInvariant>(ServiceLifetime.Transient);
        builder.Register<IWebCultureManager, WebCultureManager>(ServiceLifetime.Scoped);
        //builder.Register<ICultureSelector, SiteCultureSelector>(ServiceLifetime.Singleton);
        builder.Register<ICultureSelector, CookieCultureSelector>(ServiceLifetime.Singleton);

        // User Profile Providers
        builder.Register<IUserProfileProvider, AccountUserProfileProvider>(ServiceLifetime.Singleton);
        builder.Register<IUserProfileProvider, ThemeUserProfileProvider>(ServiceLifetime.Singleton);

        // Data / Services
        builder.Register<IGenericAttributeService, GenericAttributeService>(ServiceLifetime.Scoped);

        // Rendering
        builder.Register<IRazorViewRenderService, MantleRazorViewRenderService>(ServiceLifetime.Transient);

        builder.Register<IODataRegistrar, ODataRegistrar>(ServiceLifetime.Singleton);

        // Embedded File Provider
        builder.Register<IEmbeddedFileProviderRegistrar, EmbeddedFileProviderRegistrar>(ServiceLifetime.Scoped);

        builder.Register<IMantleHtmlHelper, MantleHtmlHelper>(ServiceLifetime.Scoped);
    }

    public virtual int Order => 0;
}