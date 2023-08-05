using Mantle.Web.Infrastructure;

namespace Mantle.Web.Messaging.Infrastructure;

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
                    ModuleId = "viewmodels/admin/messaging/templates",
                    Route = "messaging/templates",
                    JsPath = "/_content/Mantle.Web.Messaging/js/message-templates",
                    Title = T[LocalizableStrings.MessageTemplates].Value
                },

                new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/messaging/queued-email",
                    Route = "messaging/queued-email",
                    JsPath = "/_content/Mantle.Web.Messaging/js/queued-emails",
                    Title = T[LocalizableStrings.QueuedEmails].Value
                }
            };

            return routes;
        }
    }
}