using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Web;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace MantleCMS.Areas.Admin
{
    public class AdminDurandalRouteProvider : IDurandalRouteProvider
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
                    ModuleId = "viewmodels/admin/dashboard",
                    Route = "",
                    JsPath = "viewmodels/admin/dashboard",
                    Title = "Dashboard"
                });

                return routes;
            }
        }

        #endregion IDurandalRouteProvider Members
    }
}