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

        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("get-translations")]
        public JsonResult GetTranslations()
        {
            return Json(new
            {
                Set = T[MantleWebLocalizableStrings.General.Set].Value,
                SetDesktopThemeError = T[MantleWebLocalizableStrings.Themes.SetDesktopThemeError].Value,
                SetDesktopThemeSuccess = T[MantleWebLocalizableStrings.Themes.SetDesktopThemeSuccess].Value,
                SetMobileThemeError = T[MantleWebLocalizableStrings.Themes.SetMobileThemeError].Value,
                SetMobileThemeSuccess = T[MantleWebLocalizableStrings.Themes.SetMobileThemeSuccess].Value,
                Columns = new
                {
                    IsDefaultTheme = T[MantleWebLocalizableStrings.Themes.Model.IsDefaultTheme].Value,
                    PreviewImageUrl = T[MantleWebLocalizableStrings.Themes.Model.PreviewImageUrl].Value,
                    SupportRtl = T[MantleWebLocalizableStrings.Themes.Model.SupportRtl].Value
                }
            });
        }
    }
}