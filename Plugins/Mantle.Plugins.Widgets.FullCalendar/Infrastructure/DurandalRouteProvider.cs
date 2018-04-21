using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Web;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Plugins.Widgets.FullCalendar.Infrastructure
{
    public class DurandalRouteProvider : IDurandalRouteProvider
    {
        #region IDurandalRouteProvider Members

        public string Area => MantleWebConstants.Areas.Admin;

        public IEnumerable<DurandalRoute> Routes
        {
            get
            {
                var T = EngineContext.Current.Resolve<IStringLocalizer>();
                var routes = new List<DurandalRoute>();

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/plugins/widgets/fullcalendar",
                    Route = "plugins/widgets/fullcalendar",
                    JsPath = "/Plugins/Widgets.FullCalendar/Scripts/index",
                    Title = T[LocalizableStrings.FullCalendar]
                });

                return routes;
            }
        }

        #endregion IDurandalRouteProvider Members
    }
}