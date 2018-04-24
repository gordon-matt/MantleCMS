using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.Common.Infrastructure
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
                    ModuleId = "/Mantle.Web.Common.Areas.Admin.Regions.Scripts.index",
                    Route = "regions",
                    Title = localizer[LocalizableStrings.Regions.Title]
                });

                return routes;
            }
        }

        public IDictionary<string, string> ModuleIdToViewUrlMappings => new Dictionary<string, string>
        {
            { "/Mantle.Web.Common.Areas.Admin.Regions.Scripts.index", "admin/regions" },
        };

        #endregion IAureliaRouteProvider Members
    }
}