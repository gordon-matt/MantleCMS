using Mantle.Web.Mvc.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Mantle.Web.ContentManagement
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routes)
        {
            // register CMS pages route
            //routes.MapRoute(
            //    name: "CmsRoute",
            //    template: "{*slug}",
            //    defaults: new { controller = "PageContent", action = "Index", area = CmsConstants.Areas.Pages, slug = string.Empty }
            //    //constraints: new { slug = new CmsRouteConstraint() }
            //);
        }

        public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            // register CMS pages route
            //endpoints.MapControllerRoute(
            //    name: "CmsRoute",
            //    pattern: "{*slug}",
            //    defaults: new { controller = "PageContent", action = "Index", area = CmsConstants.Areas.Pages, slug = string.Empty }
            //    //constraints: new { slug = new CmsRouteConstraint() }
            //);
        }

        public int Priority
        {
            get
            {
                // make sure CMS pages route gets done last
                return int.MaxValue;
            }
        }
    }
}