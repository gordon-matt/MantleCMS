using Autofac;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Plugins.Widgets.FullCalendar.ContentBlocks;
using Mantle.Plugins.Widgets.FullCalendar.Services;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Mantle.Web.Infrastructure;
using Mantle.Web.Mvc.Themes;
using Mantle.Web.Navigation;
using Mantle.Plugins;
using Mantle.Web.Security.Membership.Permissions;

namespace Mantle.Plugins.Widgets.FullCalendar.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar<ContainerBuilder>
    {
        #region IDependencyRegistrar<ContainerBuilder> Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
            {
                return;
            }

            builder.RegisterType<DurandalRouteProvider>().As<IDurandalRouteProvider>().SingleInstance();

            builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();

            builder.RegisterType<FullCalendarPermissions>().As<IPermissionProvider>().SingleInstance();
            builder.RegisterType<LocationFormatProvider>().As<ILocationFormatProvider>().SingleInstance();
            builder.RegisterType<NavigationProvider>().As<INavigationProvider>().SingleInstance();
            builder.RegisterType<ODataRegistrar>().As<IODataRegistrar>().SingleInstance();

            builder.RegisterType<FullCalendarBlock>().As<IContentBlock>().InstancePerDependency();

            builder.RegisterType<CalendarService>().As<ICalendarService>().InstancePerDependency();
            builder.RegisterType<CalendarEventService>().As<ICalendarEventService>().InstancePerDependency();
        }

        public int Order
        {
            get { return 9999; }
        }

        #endregion IDependencyRegistrar<ContainerBuilder> Members
    }
}