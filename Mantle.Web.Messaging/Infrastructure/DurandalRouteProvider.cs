using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.Messaging.Infrastructure
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
                    ModuleId = "viewmodels/admin/messaging/templates",
                    Route = "messaging/templates",
                    JsPath = "/Mantle.Web.Messaging.Scripts.messageTemplates",
                    Title = T[LocalizableStrings.MessageTemplates].Value
                });

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/messaging/queued-email",
                    Route = "messaging/queued-email",
                    JsPath = "/Mantle.Web.Messaging.Scripts.queuedEmails",
                    Title = T[LocalizableStrings.QueuedEmails].Value
                });

                return routes;
            }
        }

        #endregion IDurandalRouteProvider Members
    }
}