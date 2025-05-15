using Extenso.AspNetCore.OData;
using Mantle.Localization;
using Mantle.Web.Common.Areas.Admin.Regions.Services;
using Mantle.Web.Infrastructure;
using Mantle.Web.Navigation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.Common.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(IContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        builder.Register<IDurandalRouteProvider, DurandalRouteProvider>(ServiceLifetime.Singleton);
        builder.Register<IRequireJSConfigProvider, RequireJSConfigProvider>(ServiceLifetime.Singleton);

        builder.Register<ILanguagePack, LanguagePackInvariant>(ServiceLifetime.Singleton);
        builder.Register<INavigationProvider, NavigationProvider>(ServiceLifetime.Singleton);
        builder.Register<ILocationFormatProvider, LocationFormatProvider>(ServiceLifetime.Singleton);
        builder.Register<IRegionService, RegionService>(ServiceLifetime.Transient);
        builder.Register<IRegionSettingsService, RegionSettingsService>(ServiceLifetime.Transient);
        builder.Register<IPermissionProvider, Permissions>(ServiceLifetime.Singleton);

        builder.Register<IODataRegistrar, ODataRegistrar>(ServiceLifetime.Singleton);

        // Embedded File Provider
        builder.Register<IEmbeddedFileProviderRegistrar, EmbeddedFileProviderRegistrar>(ServiceLifetime.Scoped);
    }

    public int Order => 1;

    #endregion IDependencyRegistrar Members
}