using Mantle.Web.Mvc.Routing;
using Microsoft.AspNetCore.Builder;

namespace Mantle.Web.ContentManagement;

public class RouteProvider : IRouteProvider
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        //register CMS pages route
        endpoints.MapControllerRoute(
            name: "CmsRoute",
            pattern: "{*slug}",
            defaults: new { controller = "PageContent", action = "Index", area = CmsConstants.Areas.Pages, slug = string.Empty }
            //constraints: new { slug = new CmsRouteConstraint() }
        );
    }

    // make sure CMS pages route gets done last
    public int Priority => int.MaxValue;
}