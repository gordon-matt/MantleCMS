//using System.Linq;
//using Mantle.Infrastructure;
//using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Routing;

//namespace Mantle.Web.ContentManagement.Routing
//{
//    public class CmsRouteConstraint : IRouteConstraint
//    {
//        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
//        {
//            var workContext = EngineContext.Current.Resolve<IWebWorkContext>();
//            var pageVersionService = EngineContext.Current.Resolve<IPageVersionService>();

//            if (values[routeKey] != null)
//            {
//                var permalink = values[routeKey].ToString();

//                using (var connection = pageVersionService.OpenConnection())
//                {
//                    return connection.Query(x => x.Slug == permalink).Any();
//                }
//            }
//            return false;
//        }
//    }
//}