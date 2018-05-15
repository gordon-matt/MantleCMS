using System;
using System.Linq;
using Mantle.Plugins;
using Mantle.Security.Membership.Permissions;
using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Mantle.Web.Areas.Admin.Plugins.Controllers
{
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

        [Route("get-view-data")]
        public JsonResult GetViewData()
        {
            return Json(new
            {
                gridPageSize = SiteSettings.Value.DefaultGridPageSize,
                translations = new
                {
                    edit = T[MantleWebLocalizableStrings.General.Edit].Value,
                    getRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                    install = T[MantleWebLocalizableStrings.General.Install].Value,
                    installPluginSuccess = T[MantleWebLocalizableStrings.Plugins.InstallPluginSuccess].Value,
                    installPluginError = T[MantleWebLocalizableStrings.Plugins.InstallPluginError].Value,
                    uninstall = T[MantleWebLocalizableStrings.General.Uninstall].Value,
                    uninstallPluginSuccess = T[MantleWebLocalizableStrings.Plugins.UninstallPluginSuccess].Value,
                    uninstallPluginError = T[MantleWebLocalizableStrings.Plugins.UninstallPluginError].Value,
                    updateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                    updateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                    columns = new
                    {
                        group = T[MantleWebLocalizableStrings.Plugins.Model.Group].Value,
                        pluginInfo = T[MantleWebLocalizableStrings.Plugins.Model.PluginInfo].Value,
                    }
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
}