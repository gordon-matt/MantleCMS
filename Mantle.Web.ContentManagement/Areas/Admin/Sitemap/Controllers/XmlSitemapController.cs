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

        return PartialView("Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Views.XmlSitemap.Index");
    }

    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("get-translations")]
    public JsonResult GetTranslations()
    {
        return Json(new
        {
            ConfirmGenerateFile = T[MantleCmsLocalizableStrings.Sitemap.ConfirmGenerateFile].Value,
            GenerateFileSuccess = T[MantleCmsLocalizableStrings.Sitemap.GenerateFileSuccess].Value,
            GenerateFileError = T[MantleCmsLocalizableStrings.Sitemap.GenerateFileError].Value,
            ChangeFrequencies = new
            {
                Always = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Always].Value,
                Hourly = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Hourly].Value,
                Daily = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Daily].Value,
                Weekly = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Weekly].Value,
                Monthly = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Monthly].Value,
                Yearly = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Yearly].Value,
                Never = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Never].Value,
            },
            Columns = new
            {
                ChangeFrequency = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequency].Value,
                Id = T[MantleCmsLocalizableStrings.Sitemap.Model.Id].Value,
                Location = T[MantleCmsLocalizableStrings.Sitemap.Model.Location].Value,
                Priority = T[MantleCmsLocalizableStrings.Sitemap.Model.Priority].Value,
            }
        });
    }
}