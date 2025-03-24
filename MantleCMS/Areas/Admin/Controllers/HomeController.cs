using Mantle.Security.Membership.Permissions;
using Mantle.Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace MantleCMS.Areas.Admin.Controllers;

[Authorize(Roles = Constants.Roles.Administrators)]
[Area("Admin")]
[Route("admin")]
public class HomeController : MantleController
{
    private readonly IEnumerable<IDurandalRouteProvider> durandalRouteProviders;
    private readonly IEnumerable<IRequireJSConfigProvider> requireJSConfigProviders;

    public HomeController(
        IEnumerable<IDurandalRouteProvider> durandalRouteProviders,
        IEnumerable<IRequireJSConfigProvider> requireJSConfigProviders)
    {
        this.durandalRouteProviders = durandalRouteProviders;
        this.requireJSConfigProviders = requireJSConfigProviders;
    }

    [Route("")]
    public IActionResult Host() => View();

    [Route("dashboard")]
    public IActionResult Dashboard()
    {
        if (!CheckPermission(StandardPermissions.DashboardAccess))
        {
            return Unauthorized();
        }

        ViewBag.Title = T[LocalizableStrings.Dashboard.Title];

        return View();
    }

    [Route("shell")]
    public IActionResult Shell() => View();

    [Route("get-spa-routes")]
    public JsonResult GetSpaRoutes()
    {
        var routes = durandalRouteProviders.SelectMany(x => x.Routes);
        return Json(routes);
    }

    [Route("get-requirejs-config")]
    public JsonResult GetRequireJsConfig()
    {
        var config = new RequireJsConfig
        {
            Paths = [],
            Shim = []
        };

        // Routes First
        var routes = durandalRouteProviders.SelectMany(x => x.Routes);

        foreach (var route in routes)
        {
            config.Paths.Add(route.ModuleId, route.JsPath);
        }

        // Then Others
        foreach (var provider in requireJSConfigProviders)
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

public struct RequireJsConfig
{
    public Dictionary<string, string> Paths { get; set; }

    public Dictionary<string, string[]> Shim { get; set; }
}