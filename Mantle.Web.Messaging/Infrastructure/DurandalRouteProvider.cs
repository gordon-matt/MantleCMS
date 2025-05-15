using Mantle.Web.Infrastructure;

namespace Mantle.Web.Messaging.Infrastructure;

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
                    ModuleId = "viewmodels/admin/messaging/templates",
                    Route = "messaging/templates",
                    JsPath = "/_content/MantleFramework.Web.Messaging/js/message-templates",
                    Title = T[LocalizableStrings.MessageTemplates].Value
                },

                new()
                {
                    ModuleId = "viewmodels/admin/messaging/queued-email",
                    Route = "messaging/queued-email",
                    JsPath = "/_content/MantleFramework.Web.Messaging/js/queued-emails",
                    Title = T[LocalizableStrings.QueuedEmails].Value
                }
            };

            return routes;
        }
    }
}