namespace Mantle.Web.Areas.Admin.Plugins.Controllers;

[Authorize]
[Area(MantleWebConstants.Areas.Plugins)]
[Route("admin/plugins")]
public class PluginController : MantleController
{
    private readonly Lazy<IPluginFinder> pluginFinder;
    private readonly Lazy<IWebHelper> webHelper;

    public PluginController(Lazy<IPluginFinder> pluginFinder, Lazy<IWebHelper> webHelper)
    {
        this.pluginFinder = pluginFinder;
        this.webHelper = webHelper;
    }

    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public ActionResult Index()
    {
        if (!CheckPermission(StandardPermissions.FullAccess))
        {
            return Unauthorized();
        }

        WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.Plugins.Title]);

        ViewBag.Title = T[MantleWebLocalizableStrings.Plugins.Title];
        ViewBag.SubTitle = T[MantleWebLocalizableStrings.Plugins.ManagePlugins];

        return PartialView("Mantle.Web.Areas.Admin.Plugins.Views.Plugin.Index");
    }

    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("get-translations")]
    public JsonResult GetTranslations()
    {
        return Json(new
        {
            Edit = T[MantleWebLocalizableStrings.General.Edit].Value,
            GetRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
            Install = T[MantleWebLocalizableStrings.General.Install].Value,
            InstallPluginSuccess = T[MantleWebLocalizableStrings.Plugins.InstallPluginSuccess].Value,
            InstallPluginError = T[MantleWebLocalizableStrings.Plugins.InstallPluginError].Value,
            Uninstall = T[MantleWebLocalizableStrings.General.Uninstall].Value,
            UninstallPluginSuccess = T[MantleWebLocalizableStrings.Plugins.UninstallPluginSuccess].Value,
            UninstallPluginError = T[MantleWebLocalizableStrings.Plugins.UninstallPluginError].Value,
            UpdateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
            UpdateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
            Columns = new
            {
                Group = T[MantleWebLocalizableStrings.Plugins.Model.Group].Value,
                PluginInfo = T[MantleWebLocalizableStrings.Plugins.Model.PluginInfo].Value,
            }
        });
    }

    [HttpPost]
    [Route("install/{systemName}")]
    public JsonResult Install(string systemName)
    {
        systemName = systemName.Replace('-', '.');

        if (!CheckPermission(MantleWebPermissions.PluginsManage))
        {
            return Json(new { Success = false, Message = "Unauthorized" });
            //return new HttpUnauthorizedResult();
        }

        try
        {
            var pluginDescriptor = pluginFinder.Value.GetPluginDescriptors(LoadPluginsMode.All)
                .FirstOrDefault(x => x.SystemName.Equals(systemName, StringComparison.OrdinalIgnoreCase));

            if (pluginDescriptor == null)
            {
                //No plugin found with the specified id
                return Json(new { Success = false, Message = "Plugin Not Found" });
                //return RedirectToAction("Index");
            }

            //check whether plugin is not installed
            if (pluginDescriptor.Installed)
            {
                return Json(new { Success = false, Message = "Plugin Not Installed" });
                //return RedirectToAction("Index");
            }

            //install plugin
            pluginDescriptor.Instance().Install();

            //restart application
            webHelper.Value.RestartAppDomain();
        }
        catch (Exception x)
        {
            Logger.LogError(new EventId(), x, x.GetBaseException().Message);
            return Json(new { Success = false, Message = x.GetBaseException().Message });
        }

        return Json(new { Success = true, Message = T[MantleWebLocalizableStrings.Plugins.InstallPluginSuccess].Value });
        //return RedirectToAction("Index");
    }

    [HttpPost]
    [Route("uninstall/{systemName}")]
    public JsonResult Uninstall(string systemName)
    {
        systemName = systemName.Replace('-', '.');

        if (!CheckPermission(MantleWebPermissions.PluginsManage))
        {
            return Json(new { Success = false, Message = "Unauthorized" });
            //return new HttpUnauthorizedResult();
        }

        try
        {
            var pluginDescriptor = pluginFinder.Value.GetPluginDescriptors(LoadPluginsMode.All)
                .FirstOrDefault(x => x.SystemName.Equals(systemName, StringComparison.OrdinalIgnoreCase));

            if (pluginDescriptor == null)
            {
                //No plugin found with the specified id
                return Json(new { Success = false, Message = "Plugin Not Found" });
                //return RedirectToAction("Index");
            }

            //check whether plugin is installed
            if (!pluginDescriptor.Installed)
            {
                return Json(new { Success = false, Message = "Plugin Not Installed" });
                //return RedirectToAction("Index");
            }

            //uninstall plugin
            pluginDescriptor.Instance().Uninstall();

            //restart application
            webHelper.Value.RestartAppDomain();
        }
        catch (Exception x)
        {
            Logger.LogError(new EventId(), x, x.GetBaseException().Message);
            return Json(new { Success = false, Message = x.GetBaseException().Message });
        }

        return Json(new { Success = true, Message = T[MantleWebLocalizableStrings.Plugins.UninstallPluginSuccess].Value });
        //return RedirectToAction("Index");
    }
}