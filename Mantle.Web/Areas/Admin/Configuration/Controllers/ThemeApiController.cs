using System;
using System.Linq;
using System.Threading.Tasks;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Mantle.Web.Areas.Admin.Configuration.Models;
using Mantle.Web.Configuration;
using Mantle.Web.Configuration.Services;
using Mantle.Web.Mvc.KendoUI;
using Mantle.Web.Mvc.Themes;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Areas.Admin.Configuration.Controllers.Api
{
    [Area(MantleWebConstants.Areas.Configuration)]
    [Route("api/configuration/themes")]
    public class ThemeApiController : Controller
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IThemeProvider themeProvider;
        private readonly IWorkContext workContext;
        private readonly SiteSettings siteSettings;
        private readonly Lazy<ISettingService> settingsService;

        public ThemeApiController(
            IAuthorizationService authorizationService,
            IThemeProvider themeProvider,
            IWorkContext workContext,
            SiteSettings siteSettings,
            Lazy<ISettingService> settingsService)
        {
            this.authorizationService = authorizationService;
            this.settingsService = settingsService;
            this.siteSettings = siteSettings;
            this.themeProvider = themeProvider;
            this.workContext = workContext;
        }

        [HttpPost]
        [Route("get")]
        public virtual async Task<IActionResult> Get([FromBody]KendoGridMvcRequest request)
        {
            if (!CheckPermission(MantleWebPermissions.ThemesRead))
            {
                return Unauthorized();
            }

            var themes = themeProvider.GetThemeConfigurations()
                .Select(x => (EdmThemeConfiguration)x)
                .ToList();

            foreach (var theme in themes)
            {
                if (theme.Title == siteSettings.DefaultTheme)
                {
                    theme.IsDefaultTheme = true;
                }
            }

            var query = themes.AsQueryable();
            var grid = new CustomKendoGridEx<EdmThemeConfiguration>(request, query);
            return Json(grid);
        }

        [HttpPost]
        [Route("Default.SetTheme")]
        public virtual void SetTheme([FromBody]dynamic data)
        {
            // TODO: Change return type to IHttpResult and return UnauthorizedResult
            if (!CheckPermission(MantleWebPermissions.ThemesWrite))
            {
                return;
            }

            string themeName = data.themeName;
            var themeConfig = themeProvider.GetThemeConfiguration(themeName);

            siteSettings.DefaultTheme = themeName;

            if (!string.IsNullOrEmpty(themeConfig.DefaultLayoutPath))
            {
                siteSettings.DefaultFrontendLayoutPath = themeConfig.DefaultLayoutPath;
            }
            else
            {
                siteSettings.DefaultFrontendLayoutPath = "~/Views/Shared/_Layout.cshtml";
            }

            settingsService.Value.SaveSettings(siteSettings, workContext.CurrentTenant.Id);
            MantleWebConstants.ResetCache();
        }

        protected bool CheckPermission(Permission permission)
        {
            return authorizationService.TryCheckAccess(permission, workContext.CurrentUser);
        }
    }
}