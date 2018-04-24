using System.Collections.Generic;
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
                    ModuleId = "/Mantle.Web.Messaging.Scripts.messageTemplates",
                    Route = "messaging/templates",
                    Title = T[LocalizableStrings.MessageTemplates].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/Mantle.Web.Messaging.Scripts.queuedEmails",
                    Route = "messaging/queued-email",
                    Title = T[LocalizableStrings.QueuedEmails].Value
                });

                return routes;
            }
        }

        public IDictionary<string, string> ModuleIdToViewUrlMappings => new Dictionary<string, string>
        {
            { "/Mantle.Web.Messaging.Scripts.messageTemplates", "admin/messaging/templates" },
            { "/Mantle.Web.Messaging.Scripts.queuedEmails", "admin/messaging/queued-email" },
        };

        #endregion IAureliaRouteProvider Members
    }
}