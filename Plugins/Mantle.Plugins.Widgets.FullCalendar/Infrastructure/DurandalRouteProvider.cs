using Mantle.Web.Infrastructure;

namespace Mantle.Plugins.Widgets.FullCalendar.Infrastructure;

public class DurandalRouteProvider : IDurandalRouteProvider
{
    public IEnumerable<DurandalRoute> Routes
    {
        get
        {
            var T = DependoResolver.Instance.Resolve<IStringLocalizer>();
            var routes = new List<DurandalRoute>
            {
                new()
                {
                    ModuleId = "viewmodels/plugins/widgets/fullcalendar",
                    Route = "plugins/widgets/fullcalendar",
                    JsPath = "/_content/Mantle.Plugins.Widgets.FullCalendar/js/index",
                    Title = T[LocalizableStrings.FullCalendar]
                }
            };

            return routes;
        }
    }
}