using Mantle.Infrastructure;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.Messaging.Infrastructure
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
                    ModuleId = "/aurelia-app/embedded/Mantle.Web.Messaging.Scripts.MessageTemplates.index",
                    Route = "messaging/templates",
                    Name = "mantle-web/messaging/templates",
                    Title = T[LocalizableStrings.MessageTemplates].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/aurelia-app/embedded/Mantle.Web.Messaging.Scripts.QueuedEmails.index",
                    Route = "messaging/queued-email",
                    Name = "mantle-web/messaging/queued-email",
                    Title = T[LocalizableStrings.QueuedEmails].Value
                });

                return routes;
            }
        }

        public IDictionary<string, string> ModuleIdToViewUrlMappings => new Dictionary<string, string>
        {
            { "aurelia-app/embedded/Mantle.Web.Messaging.Scripts.MessageTemplates.index", "admin/messaging/templates" },
            { "aurelia-app/embedded/Mantle.Web.Messaging.Scripts.QueuedEmails.index", "admin/messaging/queued-email" },
        };

        #endregion IAureliaRouteProvider Members
    }
}