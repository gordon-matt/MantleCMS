using Mantle.Infrastructure;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Plugins.Widgets.FullCalendar.Infrastructure
{
    public class DurandalRouteProvider : IDurandalRouteProvider
    {
        public IEnumerable<DurandalRoute> Routes
        {
            get
            {
                var T = EngineContext.Current.Resolve<IStringLocalizer>();
                var routes = new List<DurandalRoute>
                {
                    new DurandalRoute
                    {
                        ModuleId = "viewmodels/plugins/widgets/fullcalendar",
                        Route = "plugins/widgets/fullcalendar",
                        JsPath = "/durandal-app/embedded/Mantle.Plugins.Widgets.FullCalendar.wwwroot.js.index",
                        Title = T[LocalizableStrings.FullCalendar]
                    }
                };

                return routes;
            }
        }
    }
}