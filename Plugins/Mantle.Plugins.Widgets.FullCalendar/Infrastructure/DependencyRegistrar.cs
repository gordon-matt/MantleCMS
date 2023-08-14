using Autofac;
using Extenso.AspNetCore.OData;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Plugins.Widgets.FullCalendar.ContentBlocks;
using Mantle.Plugins.Widgets.FullCalendar.Services;
using Mantle.Security.Membership.Permissions;
using Mantle.Web.Configuration;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Mantle.Web.Infrastructure;
using Mantle.Web.Mvc.Themes;
using Mantle.Web.Navigation;
using Microsoft.Extensions.Configuration;

namespace Mantle.Plugins.Widgets.FullCalendar.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
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
            builder.RegisterType<FullCalendarPluginSettings>().As<ISettings>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 9999; }
        }

        #endregion IDependencyRegistrar Members
    }
}