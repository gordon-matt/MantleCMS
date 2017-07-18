using System;
using System.Collections.Generic;
using System.Linq;
using MantleCMS.Models;
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
        private readonly Lazy<IEnumerable<IDurandalRouteProvider>> durandalRouteProviders;
        private readonly Lazy<IEnumerable<IRequireJSConfigProvider>> requireJSConfigProviders;

        public HomeController(
            Lazy<IEnumerable<IDurandalRouteProvider>> durandalRouteProviders,
            Lazy<IEnumerable<IRequireJSConfigProvider>> requireJSConfigProviders)
        {
            this.durandalRouteProviders = durandalRouteProviders;
            this.requireJSConfigProviders = requireJSConfigProviders;
        }

        [Route("")]
        public IActionResult Host()
        {
            return View();
        }

        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            ViewBag.Title = "Dashboard";

            return View();
        }

        [Route("shell")]
        public IActionResult Shell()
        {
            return View();
        }

        [Route("get-spa-routes")]
        public JsonResult GetSpaRoutes()
        {
            var routes = durandalRouteProviders.Value
                .Where(x => x.Area == "Admin")
                .SelectMany(x => x.Routes);
            return Json(routes);
        }

        [Route("get-requirejs-config")]
        public JsonResult GetRequireJsConfig()
        {
            var config = new RequireJsConfig
            {
                Paths = new Dictionary<string, string>(),
                Shim = new Dictionary<string, string[]>()
            };

            // Routes First
            var routes = durandalRouteProviders.Value
                .Where(x => x.Area == "Admin")
                .SelectMany(x => x.Routes);

            foreach (var route in routes)
            {
                config.Paths.Add(route.ModuleId, route.JsPath);
            }

            // Then Others
            foreach (var provider in requireJSConfigProviders.Value)
            {
                foreach (var pair in provider.Paths)
                {
                    if (!config.Paths.ContainsKey(pair.Key))
                    {
                        config.Paths.Add(pair.Key, pair.Value);
                    }
                }
                foreach (var pair in provider.Shim)
                {
                    if (!config.Shim.ContainsKey(pair.Key))
                    {
                        config.Shim.Add(pair.Key, pair.Value);
                    }
                }
            }

            return Json(config);
        }
    }
}