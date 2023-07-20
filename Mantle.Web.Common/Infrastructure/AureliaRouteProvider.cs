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
                    ModuleId = "/aurelia-app/embedded/Mantle.Web.Common.Areas.Admin.Regions.Scripts.index",
                    Route = "regions",
                    Name = "mantle-web-common/regions",
                    Title = localizer[LocalizableStrings.Regions.Title]
                });

                return routes;
            }
        }

        public IDictionary<string, string> ModuleIdToViewUrlMappings => new Dictionary<string, string>
        {
            { "aurelia-app/embedded/Mantle.Web.Common.Areas.Admin.Regions.Scripts.index", "admin/regions" },
        };

        #endregion IAureliaRouteProvider Members
    }
}