using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Web;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Plugins.Widgets.FullCalendar.Infrastructure
{
    public class AureliaRouteProvider : IAureliaRouteProvider
    {
        #region IAureliaRouteProvider Members

        public string Area => MantleWebConstants.Areas.Admin;

        public IEnumerable<AureliaRoute> Routes
        {
            get
            {
                var T = EngineContext.Current.Resolve<IStringLocalizer>();
                var routes = new List<AureliaRoute>();

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/aurelia-app/Plugins/Widgets.FullCalendar/wwwroot/js/index",
                    Route = "plugins/widgets/fullcalendar",
                    Name = "plugins/widgets/fullcalendar",
                    Title = T[LocalizableStrings.FullCalendar]
                });

                return routes;
            }
        }

        public IDictionary<string, string> ModuleIdToViewUrlMappings => new Dictionary<string, string>
        {
            { "aurelia-app/Plugins/Widgets.FullCalendar/Scripts/index", "admin/plugins/widgets/fullcalendar" },
        };

        #endregion IAureliaRouteProvider Members
    }
}