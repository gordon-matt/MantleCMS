namespace Mantle.Web.Areas.Admin.Log.Controllers;

[Authorize]
[Area(MantleWebConstants.Areas.Log)]
[Route("admin/log")]
public class LogController : MantleController
{
    //[Compress]
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public ActionResult Index()
    {
        if (!CheckPermission(MantleWebPermissions.LogRead))
        {
            return Unauthorized();
        }

        WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.Log.Title]);
        ViewBag.Title = T[MantleWebLocalizableStrings.Log.Title];

        return PartialView();
    }
}