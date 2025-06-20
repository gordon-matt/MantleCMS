﻿namespace Mantle.Web.Areas.Admin.Configuration.Controllers;

[Authorize]
[Area(MantleWebConstants.Areas.Configuration)]
[Route("admin/configuration/themes")]
public class ThemeController : MantleController
{
    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index() => !CheckPermission(MantleWebPermissions.ThemesRead) ? Unauthorized() : PartialView();
}