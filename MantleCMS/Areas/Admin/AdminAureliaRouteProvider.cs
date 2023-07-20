using Mantle.Web;
using Mantle.Web.Infrastructure;

namespace MantleCMS.Areas.Admin
{
    public class AdminAureliaRouteProvider : IAureliaRouteProvider
    {
        #region IAureliaRouteProvider Members

        public string Area => MantleWebConstants.Areas.Admin;

        public IEnumerable<AureliaRoute> Routes
        {
            get
            {
                //var T = EngineContext.Current.Resolve<IStringLocalizer>();
                var routes = new List<AureliaRoute>();

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/aurelia-app/admin/dashboard",
                    Route = "",
                    Name = "home",
                    Title = "Dashboard"
                });

                return routes;
            }
        }

        public IDictionary<string, string> ModuleIdToViewUrlMappings => new Dictionary<string, string>
        {
            // HomeController
            { "aurelia-app/admin/dashboard", "admin/dashboard" },
            { "aurelia-app/admin/app", "admin/app" }
        };

        #endregion IAureliaRouteProvider Members
    }
}