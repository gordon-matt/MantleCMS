using Mantle.Infrastructure;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.Messaging.Infrastructure
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
                        ModuleId = "viewmodels/admin/messaging/templates",
                        Route = "messaging/templates",
                        JsPath = "/durandal-app/embedded/Mantle.Web.Messaging.Scripts.messageTemplates",
                        Title = T[LocalizableStrings.MessageTemplates].Value
                    },

                    new DurandalRoute
                    {
                        ModuleId = "viewmodels/admin/messaging/queued-email",
                        Route = "messaging/queued-email",
                        JsPath = "/durandal-app/embedded/Mantle.Web.Messaging.Scripts.queuedEmails",
                        Title = T[LocalizableStrings.QueuedEmails].Value
                    }
                };

                return routes;
            }
        }
    }
}