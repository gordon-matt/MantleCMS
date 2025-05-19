using Mantle.Tenants.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.Media.Controllers;

[Authorize]
[Area(CmsConstants.Areas.Media)]
[Route("admin/media")]
public class MediaController : MantleController
{
    private readonly ITenantService tenantService;

    public MediaController(ITenantService tenantService)
    {
        this.tenantService = tenantService;
    }

    [Route("")]
    public IActionResult Index()
    {
        tenantService.EnsureTenantMediaFolderExists(WorkContext.CurrentTenant.Id);

        return !CheckPermission(CmsPermissions.MediaRead) ? Unauthorized() : PartialView();
    }
}