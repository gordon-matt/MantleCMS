﻿using Mantle.Web.Infrastructure;

namespace MantleCMS.Areas.Admin;

public class AdminDurandalRouteProvider : IDurandalRouteProvider
{
    public IEnumerable<DurandalRoute> Routes
    {
        get
        {
            //var T = DependoResolver.Instance.Resolve<IStringLocalizer>();
            var routes = new List<DurandalRoute>
            {
                new()
                {
                    ModuleId = "viewmodels/admin/dashboard",
                    //Title = "Mantle Admin",
                    Route = "",
                    JsPath = "viewmodels/admin/dashboard"
                    //Nav = true
                }
            };

            return routes;
        }
    }
}