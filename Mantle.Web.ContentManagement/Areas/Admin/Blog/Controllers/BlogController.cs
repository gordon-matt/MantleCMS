namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Controllers;

[Authorize]
[Area(CmsConstants.Areas.Blog)]
[Route("admin/blog")]
public class BlogController : MantleController
{
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index()
    {
        if (!CheckPermission(CmsPermissions.BlogRead))
        {
            return Unauthorized();
        }

        WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Blog.Title].Value);

        ViewBag.Title = T[MantleCmsLocalizableStrings.Blog.Title].Value;
        ViewBag.SubTitle = T[MantleCmsLocalizableStrings.Blog.ManageBlog].Value;

        return PartialView();
    }
}