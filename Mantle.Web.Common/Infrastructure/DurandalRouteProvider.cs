using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.Common.Infrastructure
{
    public class DurandalRouteProvider : IDurandalRouteProvider
    {
        #region IDurandalRouteProvider Members

        public string Area => MantleWebConstants.Areas.Admin;

        public IEnumerable<DurandalRoute> Routes
        {
            get
            {
                var localizer = EngineContext.Current.Resolve<IStringLocalizer>();
                var routes = new List<DurandalRoute>();

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/regions",
                    Route = "regions",
                    JsPath = "/Mantle.Web.Common.Areas.Admin.Regions.Scripts.index",
                    Title = localizer[LocalizableStrings.Regions.Title]
                });

                return routes;
            }
        }

        #endregion IDurandalRouteProvider Members
    }
}