namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Controllers;

[Authorize]
[Area(CmsConstants.Areas.Blog)]
[Route("admin/blog")]
public class BlogController : MantleController
{
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index() => !CheckPermission(CmsPermissions.BlogRead) ? Unauthorized() : PartialView();
}