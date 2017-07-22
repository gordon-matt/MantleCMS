using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Web;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Plugins.Messaging.Forums.Infrastructure
{
    public class DurandalRouteProvider : IDurandalRouteProvider
    {
        #region IDurandalRouteProvider Members

        public string Area => MantleWebConstants.Areas.Admin;

        public IEnumerable<DurandalRoute> Routes
        {
            get
            {
                var localizer = EngineContext.Current.Resolve<IStringLocalizer>();
                var routes = new List<DurandalRoute>();

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/plugins/messaging/forums",
                    Route = "plugins/messaging/forums",
                    JsPath = "/Plugins/Messaging.Forums/Scripts/index",
                    Title = localizer[LocalizableStrings.Forums]
                });

                return routes;
            }
        }

        #endregion IDurandalRouteProvider Members
    }
}