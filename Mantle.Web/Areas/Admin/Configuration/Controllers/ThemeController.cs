namespace Mantle.Web.Areas.Admin.Configuration.Controllers;

[Authorize]
[Area(MantleWebConstants.Areas.Configuration)]
[Route("admin/configuration/themes")]
public class ThemeController : MantleController
{
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index()
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
}