using Mantle.Infrastructure;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.Common.Infrastructure
{
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
                        JsPath = "/durandal-app/embedded/Mantle.Web.Common.Areas.Admin.Regions.Scripts.index",
                        Title = T[LocalizableStrings.Regions.Title]
                    }
                };

                return routes;
            }
        }
    }
}