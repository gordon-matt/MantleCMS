using System;
using System.Collections.Generic;
using System.Linq;
using Mantle.Collections;
using Mantle.Web.Areas.Admin.Configuration.Models;
using Mantle.Web.Configuration;
using Mantle.Web.Configuration.Services;
using Mantle.Web.Mvc.Themes;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Areas.Admin.Configuration.Controllers.Api
{
    public class ThemeApiController : ODataController
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

        // GET: odata/kore/cms/Plugins
        //[EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
        public virtual IEnumerable<EdmThemeConfiguration> Get(ODataQueryOptions<EdmThemeConfiguration> options)
        {
            if (!CheckPermission(MantleWebPermissions.ThemesRead))
            {
                return Enumerable.Empty<EdmThemeConfiguration>().AsQueryable();
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

            var settings = new ODataValidationSettings()
            {
                AllowedQueryOptions = AllowedQueryOptions.All
            };
            options.Validate(settings);

            var results = options.ApplyTo(themes.AsQueryable());
            return (results as IQueryable<EdmThemeConfiguration>).ToHashSet();
        }

        [HttpPost]
        public virtual void SetTheme(ODataActionParameters parameters)
        {
            // TODO: Change return type to IHttpResult and return UnauthorizedResult
            if (!CheckPermission(MantleWebPermissions.ThemesWrite))
            {
                return;
            }

            string themeName = (string)parameters["themeName"];
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