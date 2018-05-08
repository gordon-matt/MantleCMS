using Autofac;
using Mantle.Caching;
using Mantle.Helpers;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Web.Areas.Admin;
using Mantle.Web.Configuration;
using Mantle.Web.Configuration.Services;
using Mantle.Web.Helpers;
using Mantle.Web.Localization;
using Mantle.Web.Localization.Services;
using Mantle.Web.Mvc.EmbeddedResources;
using Mantle.Web.Mvc.Rendering;
using Mantle.Web.Mvc.Resources;
using Mantle.Web.Mvc.Routing;
using Mantle.Web.Mvc.Themes;
using Mantle.Web.Navigation;
using Mantle.Plugins;
using Mantle.Web.Security.Membership;
using Mantle.Web.Security.Membership.Permissions;

namespace Mantle.Web.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            // Helpers
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerLifetimeScope();

            // Plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerLifetimeScope();
            
            // Work Context, Themes, Routing, etc
            builder.RegisterType<WorkContext>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ThemeProvider>().As<IThemeProvider>().InstancePerLifetimeScope();
            builder.RegisterType<ThemeContext>().As<IThemeContext>().InstancePerLifetimeScope();
            builder.RegisterType<EmbeddedResourceResolver>().As<IEmbeddedResourceResolver>().SingleInstance();
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();

            // Resources (JS and CSS)
            builder.RegisterType<ScriptRegistrar>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<StyleRegistrar>().AsSelf().InstancePerLifetimeScope();
            //builder.RegisterType<ResourcesManager>().As<IResourcesManager>().InstancePerLifetimeScope();

            // Security
            builder.RegisterType<RolesBasedAuthorizationService>().As<IAuthorizationService>().SingleInstance();

            // Configuration
            builder.RegisterModule<ConfigurationModule>();
            builder.RegisterType<DefaultSettingService>().As<ISettingService>();
            builder.RegisterType<SiteSettings>().As<ISettings>().InstancePerLifetimeScope();
            builder.RegisterType<MembershipSettings>().As<ISettings>().InstancePerLifetimeScope();

            // Navigation
            builder.RegisterType<AureliaRouteProvider>().As<IAureliaRouteProvider>().SingleInstance();
            builder.RegisterType<NavigationManager>().As<INavigationManager>().InstancePerDependency();
            builder.RegisterType<NavigationProvider>().As<INavigationProvider>().SingleInstance();

            // Permission Providers
            builder.RegisterType<StandardPermissions>().As<IPermissionProvider>().SingleInstance();
            builder.RegisterType<MantleWebPermissions>().As<IPermissionProvider>().SingleInstance();

            // Work Context State Providers
            builder.RegisterType<CurrentUserStateProvider>().As<IWorkContextStateProvider>();
            builder.RegisterType<CurrentThemeStateProvider>().As<IWorkContextStateProvider>();
            builder.RegisterType<CurrentCultureCodeStateProvider>().As<IWorkContextStateProvider>();

            // Localization
            builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().InstancePerDependency();
            builder.RegisterType<WebCultureManager>().AsImplementedInterfaces().InstancePerLifetimeScope();
            //builder.RegisterType<SiteCultureSelector>().As<ICultureSelector>().SingleInstance();
            builder.RegisterType<CookieCultureSelector>().As<ICultureSelector>().SingleInstance();

            // User Profile Providers
            builder.RegisterType<AccountUserProfileProvider>().As<IUserProfileProvider>().SingleInstance();
            builder.RegisterType<ThemeUserProfileProvider>().As<IUserProfileProvider>().SingleInstance();

            // Data / Services
            builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().InstancePerLifetimeScope();

            // Rendering
            builder.RegisterType<RazorViewRenderService>().As<IRazorViewRenderService>().SingleInstance();

            builder.RegisterType<ODataRegistrar>().As<IODataRegistrar>().SingleInstance();
        }

        public int Order
        {
            get { return 0; }
        }

        #endregion IDependencyRegistrar Members
    }
}