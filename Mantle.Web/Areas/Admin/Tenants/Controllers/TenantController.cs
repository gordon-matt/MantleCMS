namespace Mantle.Web.Areas.Admin.Tenants.Controllers;

[Authorize]
[Area(MantleWebConstants.Areas.Tenants)]
[Route("admin/tenants")]
public class TenantController : MantleController
{
    [Route("")]
    public IActionResult Index()
    {
        if (!CheckPermission(StandardPermissions.FullAccess))
        {
            return Unauthorized();
        }

        WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.Tenants.Title].Value);

        ViewBag.Title = T[MantleWebLocalizableStrings.Tenants.Title].Value;
        ViewBag.SubTitle = T[MantleWebLocalizableStrings.Tenants.ManageTenants].Value;

        return PartialView();
    }
}