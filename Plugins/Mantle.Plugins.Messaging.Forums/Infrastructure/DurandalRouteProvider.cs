using Mantle.Web.Infrastructure;

namespace Mantle.Plugins.Messaging.Forums.Infrastructure;

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
                    ModuleId = "viewmodels/plugins/messaging/forums",
                    Route = "plugins/messaging/forums",
                    JsPath = "/_content/Mantle.Plugins.Messaging.Forums/js/index",
                    Title = T[LocalizableStrings.Forums]
                }
            };

            return routes;
        }
    }
}