namespace Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Controllers;

[Authorize]
[Area(CmsConstants.Areas.Sitemap)]
[Route("admin/sitemap/xml-sitemap")]
public class XmlSitemapController : MantleController
{
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index()
    {
        if (!CheckPermission(CmsPermissions.SitemapRead))
        {
            return Unauthorized();
        }

        WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Sitemap.Title].Value);
        WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Sitemap.XMLSitemap].Value);

        ViewBag.Title = T[MantleCmsLocalizableStrings.Sitemap.XMLSitemap].Value;

        return PartialView();
    }
}