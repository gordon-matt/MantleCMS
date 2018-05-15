using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Areas.Admin.Configuration.Controllers
{
    [Authorize]
    [Area(MantleWebConstants.Areas.Configuration)]
    [Route("admin/configuration/themes")]
    public class ThemeController : MantleController
    {
        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("")]
        public ActionResult Index()
        {
            if (!CheckPermission(MantleWebPermissions.ThemesRead))
            {
                return Unauthorized();
            }

            WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.General.Configuration].Value);
            WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.General.Themes].Value);

            ViewBag.Title = T[MantleWebLocalizableStrings.General.Configuration].Value;
            ViewBag.SubTitle = T[MantleWebLocalizableStrings.General.Themes].Value;

            return PartialView();
        }

        [Route("get-view-data")]
        public JsonResult GetViewData()
        {
            return Json(new
            {
                gridPageSize = SiteSettings.Value.DefaultGridPageSize,
                translations = new
                {
                    set = T[MantleWebLocalizableStrings.General.Set].Value,
                    setDesktopThemeError = T[MantleWebLocalizableStrings.Themes.SetDesktopThemeError].Value,
                    setDesktopThemeSuccess = T[MantleWebLocalizableStrings.Themes.SetDesktopThemeSuccess].Value,
                    setMobileThemeError = T[MantleWebLocalizableStrings.Themes.SetMobileThemeError].Value,
                    setMobileThemeSuccess = T[MantleWebLocalizableStrings.Themes.SetMobileThemeSuccess].Value,
                    columns = new
                    {
                        isDefaultTheme = T[MantleWebLocalizableStrings.Themes.Model.IsDefaultTheme].Value,
                        previewImageUrl = T[MantleWebLocalizableStrings.Themes.Model.PreviewImageUrl].Value,
                        supportRtl = T[MantleWebLocalizableStrings.Themes.Model.SupportRtl].Value
                    }
                }
            });
        }
    }
}