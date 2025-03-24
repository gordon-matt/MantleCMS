namespace Mantle.Web.Areas.Admin.Tenants.Controllers;

[Authorize]
[Area(MantleWebConstants.Areas.Tenants)]
[Route("admin/tenants")]
public class TenantController : MantleController
{
    [Route("")]
    public IActionResult Index() => !CheckPermission(StandardPermissions.FullAccess)
        ? Unauthorized()
        : PartialView();
}