namespace Mantle.Plugins.Messaging.Forums.Controllers;

[Authorize]
[Route(Constants.RouteArea)]
public class ForumsAdminController : MantleController
{
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index()
    {
        if (!CheckPermission(ForumPermissions.ReadForums))
        {
            return Unauthorized();
        }

        WorkContext.Breadcrumbs.Add(T[LocalizableStrings.Forums], Url.Action("Index"));

        ViewBag.Title = T[LocalizableStrings.Forums];

        return PartialView();
    }
}