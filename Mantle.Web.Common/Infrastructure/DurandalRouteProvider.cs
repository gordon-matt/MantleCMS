﻿using Mantle.Web.Infrastructure;

namespace Mantle.Web.Common.Infrastructure;

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
                    ModuleId = "viewmodels/admin/regions",
                    Route = "regions",
                    JsPath = "/_content/Mantle.Web.Common/js/index",
                    Title = T[LocalizableStrings.Regions.Title]
                }
            };

            return routes;
        }
    }
}