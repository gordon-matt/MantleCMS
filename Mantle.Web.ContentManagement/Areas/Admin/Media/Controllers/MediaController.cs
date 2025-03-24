namespace Mantle.Web.ContentManagement.Areas.Admin.Media.Controllers;

[Authorize]
[Area(CmsConstants.Areas.Media)]
[Route("admin/media")]
public class MediaController : MantleController
{
    [Route("")]
    public IActionResult Index() => !CheckPermission(CmsPermissions.MediaRead) ? Unauthorized() : PartialView();
}