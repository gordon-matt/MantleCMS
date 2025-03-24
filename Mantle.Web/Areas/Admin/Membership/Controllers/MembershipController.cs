namespace Mantle.Web.Areas.Admin.Membership.Controllers;

[Authorize]
[Area(MantleWebConstants.Areas.Membership)]
[Route("admin/membership")]
public class MembershipController : MantleController
{
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index() => !CheckPermission(MantleWebPermissions.MembershipManage) ? Unauthorized() : PartialView();
}