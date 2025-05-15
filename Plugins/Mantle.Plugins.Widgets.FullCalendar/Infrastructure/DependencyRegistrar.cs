using Extenso.AspNetCore.OData;
using Mantle.Localization;
using Mantle.Plugins.Widgets.FullCalendar.ContentBlocks;
using Mantle.Plugins.Widgets.FullCalendar.Services;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Mantle.Web.Infrastructure;
using Mantle.Web.Navigation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Plugins.Widgets.FullCalendar.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(IContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
        {
            return;
        }

        builder.Register<IDurandalRouteProvider, DurandalRouteProvider>(ServiceLifetime.Singleton);

        builder.Register<ILanguagePack, LanguagePackInvariant>(ServiceLifetime.Singleton);

        builder.Register<IPermissionProvider, FullCalendarPermissions>(ServiceLifetime.Singleton);
        builder.Register<ILocationFormatProvider, LocationFormatProvider>(ServiceLifetime.Singleton);
        builder.Register<INavigationProvider, NavigationProvider>(ServiceLifetime.Singleton);
        builder.Register<IODataRegistrar, ODataRegistrar>(ServiceLifetime.Singleton);

        builder.Register<IContentBlock, FullCalendarBlock>(ServiceLifetime.Transient);

        builder.Register<ICalendarService, CalendarService>(ServiceLifetime.Transient);
        builder.Register<ICalendarEventService, CalendarEventService>(ServiceLifetime.Transient);
        builder.Register<ISettings, FullCalendarPluginSettings>(ServiceLifetime.Scoped);
    }

    public int Order => 9999;

    #endregion IDependencyRegistrar Members
}