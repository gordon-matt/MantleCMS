using Autofac;
using Extenso.AspNetCore.OData;
using Mantle.Localization;
using Mantle.Web.Common.Areas.Admin.Regions.Services;
using Mantle.Web.Infrastructure;
using Mantle.Web.Navigation;
using Microsoft.Extensions.Configuration;

namespace Mantle.Web.Common.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        builder.RegisterType<DurandalRouteProvider>().As<IDurandalRouteProvider>().SingleInstance();
        builder.RegisterType<RequireJSConfigProvider>().As<IRequireJSConfigProvider>().SingleInstance();

        builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();
        builder.RegisterType<NavigationProvider>().As<INavigationProvider>().SingleInstance();
        builder.RegisterType<LocationFormatProvider>().As<ILocationFormatProvider>().SingleInstance();
        builder.RegisterType<RegionService>().As<IRegionService>().InstancePerDependency();
        builder.RegisterType<RegionSettingsService>().As<IRegionSettingsService>().InstancePerDependency();
        builder.RegisterType<Permissions>().As<IPermissionProvider>().SingleInstance();

        builder.RegisterType<ODataRegistrar>().As<IODataRegistrar>().SingleInstance();

        // Embedded File Provider
        builder.RegisterType<EmbeddedFileProviderRegistrar>().As<IEmbeddedFileProviderRegistrar>().InstancePerLifetimeScope();
    }

    public int Order => 1;

    #endregion IDependencyRegistrar Members
}