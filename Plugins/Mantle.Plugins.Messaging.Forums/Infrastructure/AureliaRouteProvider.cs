using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Web;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Plugins.Messaging.Forums.Infrastructure
{
    public class AureliaRouteProvider : IAureliaRouteProvider
    {
        #region IAureliaRouteProvider Members

        public string Area => MantleWebConstants.Areas.Admin;

        public IEnumerable<AureliaRoute> Routes
        {
            get
            {
                var localizer = EngineContext.Current.Resolve<IStringLocalizer>();
                var routes = new List<AureliaRoute>();

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/aurelia-app/Plugins/Messaging.Forums/wwwroot/js/index",
                    Route = "plugins/messaging/forums",
                    Name = "plugins/messaging/forums",
                    Title = localizer[LocalizableStrings.Forums]
                });

                return routes;
            }
        }

        public IDictionary<string, string> ModuleIdToViewUrlMappings => new Dictionary<string, string>
        {
            { "aurelia-app/Plugins/Messaging.Forums/Scripts/index", "admin/plugins/messaging/forums" },
        };

        #endregion IAureliaRouteProvider Members
    }
}