namespace Mantle.Web.ContentManagement.Areas.Admin.Menus.Controllers;

[Authorize]
[Area(CmsConstants.Areas.Menus)]
[Route("admin/menus")]
public class MenuController : MantleController
{
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index()
    {
        if (!CheckPermission(CmsPermissions.MenusRead))
        {
            return Unauthorized();
        }

        return PartialView();
    }
}