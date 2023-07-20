﻿using Mantle.Web.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Mantle.Web.ContentManagement
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder endpoints)
        {
            // register CMS pages route
            //endpoints.MapRoute(
            //    name: "CmsRoute",
            //    template: "{*slug}",
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