using Autofac;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Plugins.Messaging.Forums.Services;
using Mantle.Security.Membership.Permissions;
using Mantle.Web.Configuration;
using Mantle.Web.Mvc.Themes;
using Mantle.Web.Navigation;

namespace Mantle.Plugins.Messaging.Forums.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
            {
                return;
            }

            //builder.RegisterType<AureliaRouteProvider>().As<IAureliaRouteProvider>().SingleInstance();

            builder.RegisterType<ForumSettings>().As<ISettings>().InstancePerLifetimeScope();
            builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();

            builder.RegisterType<ForumPermissions>().As<IPermissionProvider>().SingleInstance();
            builder.RegisterType<LocationFormatProvider>().As<ILocationFormatProvider>().SingleInstance();
            builder.RegisterType<NavigationProvider>().As<INavigationProvider>().SingleInstance();

            //builder.RegisterType<AutoMenuProvider>().As<IAutoMenuProvider>().SingleInstance();
            builder.RegisterType<ForumService>().As<IForumService>().InstancePerDependency();
        }

        public int Order
        {
            get { return 9999; }
        }

        #endregion IDependencyRegistrar Members
    }
}