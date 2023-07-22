using Mantle.Infrastructure;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Plugins.Messaging.Forums.Infrastructure
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
                        ModuleId = "viewmodels/plugins/messaging/forums",
                        Route = "plugins/messaging/forums",
                        JsPath = "/durandal-app/embedded/Mantle.Plugins.Messaging.Forums.wwwroot.js.index",
                        Title = T[LocalizableStrings.Forums]
                    }
                };

                return routes;
            }
        }
    }
}