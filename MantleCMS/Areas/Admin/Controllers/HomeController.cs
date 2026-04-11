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

    public HomeController(
        IEnumerable<IDurandalRouteProvider> durandalRouteProviders)
    {
        this.durandalRouteProviders = durandalRouteProviders;
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

    [Route("get-spa-routes")]
    public JsonResult GetSpaRoutes()
    {
        var routes = durandalRouteProviders.SelectMany(x => x.Routes);
        return Json(routes);
    }
}