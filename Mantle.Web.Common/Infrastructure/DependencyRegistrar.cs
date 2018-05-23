using Autofac;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Security.Membership.Permissions;
using Mantle.Web.Common.Areas.Admin.Regions.Services;
using Mantle.Web.Infrastructure;
using Mantle.Web.Mvc.Themes;
using Mantle.Web.Navigation;

namespace Mantle.Web.Common.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<AureliaRouteProvider>().As<IAureliaRouteProvider>().SingleInstance();

            builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();
            builder.RegisterType<NavigationProvider>().As<INavigationProvider>().SingleInstance();
            builder.RegisterType<LocationFormatProvider>().As<ILocationFormatProvider>().SingleInstance();
            builder.RegisterType<RegionService>().As<IRegionService>().InstancePerDependency();
            builder.RegisterType<RegionSettingsService>().As<IRegionSettingsService>().InstancePerDependency();
            builder.RegisterType<Permissions>().As<IPermissionProvider>().SingleInstance();

            builder.RegisterType<ODataRegistrar>().As<IODataRegistrar>().SingleInstance();
        }

        public int Order
        {
            get { return 1; }
        }

        #endregion IDependencyRegistrar Members
    }
}