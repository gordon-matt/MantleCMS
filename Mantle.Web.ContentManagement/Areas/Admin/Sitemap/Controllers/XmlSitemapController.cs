using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Controllers
{
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

        [Route("get-view-data")]
        public JsonResult GetViewData()
        {
            return Json(new
            {
                gridPageSize = SiteSettings.Value.DefaultGridPageSize,
                translations = new
                {
                    confirmGenerateFile = T[MantleCmsLocalizableStrings.Sitemap.ConfirmGenerateFile].Value,
                    generateFileSuccess = T[MantleCmsLocalizableStrings.Sitemap.GenerateFileSuccess].Value,
                    generateFileError = T[MantleCmsLocalizableStrings.Sitemap.GenerateFileError].Value,
                    changeFrequencies = new
                    {
                        always = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Always].Value,
                        hourly = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Hourly].Value,
                        daily = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Daily].Value,
                        weekly = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Weekly].Value,
                        monthly = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Monthly].Value,
                        yearly = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Yearly].Value,
                        never = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Never].Value,
                    },
                    columns = new
                    {
                        changeFrequency = T[MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequency].Value,
                        id = T[MantleCmsLocalizableStrings.Sitemap.Model.Id].Value,
                        location = T[MantleCmsLocalizableStrings.Sitemap.Model.Location].Value,
                        priority = T[MantleCmsLocalizableStrings.Sitemap.Model.Priority].Value,
                    }
                }
            });
        }
    }
}