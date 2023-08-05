namespace Mantle.Web.ContentManagement.Areas.Admin.Media.Controllers;

[Authorize]
[Area(CmsConstants.Areas.Media)]
[Route("admin/media")]
public class MediaController : MantleController
{
    [Route("")]
    public IActionResult Index()
    {
        if (!CheckPermission(CmsPermissions.MediaRead))
        {
            return Unauthorized();
        }

        WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Media.Title].Value);
        WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Media.ManageMedia].Value);

        ViewBag.Title = T[MantleCmsLocalizableStrings.Media.Title].Value;
        ViewBag.SubTitle = T[MantleCmsLocalizableStrings.Media.ManageMedia].Value;

        return PartialView();
    }
}