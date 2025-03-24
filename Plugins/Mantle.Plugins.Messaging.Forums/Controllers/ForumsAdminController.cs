namespace Mantle.Plugins.Messaging.Forums.Controllers;

[Authorize]
[Route(Constants.RouteArea)]
public class ForumsAdminController : MantleController
{
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index() => !CheckPermission(ForumPermissions.ReadForums) ? Unauthorized() : PartialView();
}