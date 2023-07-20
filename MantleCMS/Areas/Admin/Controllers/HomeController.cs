using Mantle.Web.Infrastructure;
using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MantleCMS.Areas.Admin.Controllers
{
    [Authorize(Roles = Constants.Roles.Administrators)]
    [Area("Admin")]
    [Route("admin")]
    public class HomeController : MantleController
    {
        private readonly IEnumerable<IAureliaRouteProvider> routeProviders;

        public HomeController(
            IEnumerable<IAureliaRouteProvider> routeProviders)
        {
            this.routeProviders = routeProviders;
        }

        [Route("")]
        public IActionResult Host()
        {
            return View();
        }

        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            return PartialView();
        }

        [Route("app")]
        public IActionResult App()
        {
            return PartialView();
        }

        //[Route("nav-bar")]
        //public IActionResult NavBar()
        //{
        //    return PartialView();
        //}

        [Route("get-spa-routes")]
        public JsonResult GetSpaRoutes()
        {
            var routes = routeProviders
                .Where(x => x.Area == "Admin")
                .SelectMany(x => x.Routes);
            return Json(routes);
        }

        [Route("get-moduleId-to-viewUrl-mappings")]
        public JsonResult GetModuleIdToViewUrlMappings()
        {
            var mappings = routeProviders
                .Where(x => x.Area == "Admin")
                .SelectMany(x => x.ModuleIdToViewUrlMappings);

            return Json(mappings);
        }
    }
}