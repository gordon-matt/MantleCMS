using Autofac;
using Mantle.Caching;
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
using Mantle.Web.Mvc.Themes;
using Mantle.Web.Navigation;
using Mantle.Web.Security.Membership;
using Mantle.Web.Security.Membership.Permissions;

namespace Mantle.Web.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar<ContainerBuilder>
    {
        #region IDependencyRegistrar<ContainerBuilder> Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();

            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("Mantle_Cache_Static").SingleInstance();

            builder.RegisterType<WebWorkContext>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ThemeProvider>().As<IThemeProvider>().InstancePerLifetimeScope();
            builder.RegisterType<ThemeContext>().As<IThemeContext>().InstancePerLifetimeScope();

            builder.RegisterType<EmbeddedResourceResolver>().As<IEmbeddedResourceResolver>().SingleInstance();

            builder.RegisterType<RolesBasedAuthorizationService>().As<IAuthorizationService>().SingleInstance();

            // configuration
            builder.RegisterModule<ConfigurationModule>();
            builder.RegisterType<DefaultSettingService>().As<ISettingService>();
            builder.RegisterType<SiteSettings>().As<ISettings>().InstancePerLifetimeScope();
            builder.RegisterType<MembershipSettings>().As<ISettings>().InstancePerLifetimeScope();

            // navigation
            builder.RegisterType<NavigationManager>().As<INavigationManager>().InstancePerDependency();
            builder.RegisterType<NavigationProvider>().As<INavigationProvider>().SingleInstance();

            // permission providers
            builder.RegisterType<StandardPermissions>().As<IPermissionProvider>().SingleInstance();
            builder.RegisterType<MantleWebPermissions>().As<IPermissionProvider>().SingleInstance();

            // work context state providers
            builder.RegisterType<CurrentUserStateProvider>().As<IWorkContextStateProvider>();
            builder.RegisterType<CurrentThemeStateProvider>().As<IWorkContextStateProvider>();
            builder.RegisterType<CurrentCultureCodeStateProvider>().As<IWorkContextStateProvider>();

            // localization
            builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().InstancePerDependency();
            builder.RegisterType<WebCultureManager>().AsImplementedInterfaces().InstancePerLifetimeScope();
            //builder.RegisterType<SiteCultureSelector>().As<ICultureSelector>().SingleInstance();
            builder.RegisterType<CookieCultureSelector>().As<ICultureSelector>().SingleInstance();

            // user profile providers
            builder.RegisterType<AccountUserProfileProvider>().As<IUserProfileProvider>().SingleInstance();
            builder.RegisterType<ThemeUserProfileProvider>().As<IUserProfileProvider>().SingleInstance();

            builder.RegisterType<DurandalRouteProvider>().As<IDurandalRouteProvider>().SingleInstance();

            builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerLifetimeScope();
            builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().InstancePerLifetimeScope();

            builder.RegisterType<RazorViewRenderService>().As<IRazorViewRenderService>().SingleInstance();
        }

        public int Order
        {
            get { return 0; }
        }

        #endregion IDependencyRegistrar<ContainerBuilder> Members
    }
}