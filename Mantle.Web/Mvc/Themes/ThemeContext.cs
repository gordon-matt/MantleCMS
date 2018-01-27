using System;
using System.Linq;
using Mantle.Infrastructure;
using Mantle.Security.Membership;
using Mantle.Threading;
using Mantle.Web.Configuration;

namespace Mantle.Web.Mvc.Themes
{
    /// <summary>
    /// Theme context
    /// </summary>
    public partial class ThemeContext : IThemeContext
    {
        private readonly IWorkContext workContext;
        private readonly IThemeProvider themeProvider;
        private readonly SiteSettings siteSettings;

        private bool isDesktopThemeCached;
        private string cachedDesktopThemeName;

        private bool isMobileThemeCached;
        //private string cachedMobileThemeName;

        public ThemeContext(
            IWorkContext workContext,
            IThemeProvider themeProvider,
            SiteSettings siteSettings)
        {
            this.workContext = workContext;
            this.themeProvider = themeProvider;
            this.siteSettings = siteSettings;
        }

        /// <summary>
        /// Get or set current theme for desktops
        /// </summary>
        public string WorkingTheme
        {
            get
            {
                if (isDesktopThemeCached)
                {
                    return cachedDesktopThemeName;
                }

                string theme = string.Empty;

                if (siteSettings.AllowUserToSelectTheme)
                {
                    if (workContext.CurrentUser != null)
                    {
                        var membershipService = EngineContext.Current.Resolve<IMembershipService>();
                        string userTheme = AsyncHelper.RunSync(() => membershipService.GetProfileEntry(workContext.CurrentUser.Id, ThemeUserProfileProvider.Fields.PreferredTheme));

                        if (!string.IsNullOrEmpty(userTheme))
                        {
                            theme = userTheme;
                        }
                    }
                }

                // Default tenant theme
                if (string.IsNullOrEmpty(theme))
                {
                    theme = siteSettings.DefaultTheme ?? "Default";
                }

                // Ensure that theme exists
                if (!themeProvider.ThemeConfigurationExists(theme))
                {
                    var themeInstance = themeProvider.GetThemeConfigurations()
                        .FirstOrDefault();

                    if (themeInstance == null)
                    {
                        throw new Exception("No theme could be loaded");
                    }

                    theme = themeInstance.ThemeName;
                }

                // Cache theme
                this.cachedDesktopThemeName = theme;
                this.isDesktopThemeCached = true;
                return theme;
            }
            set
            {
                if (!siteSettings.AllowUserToSelectTheme)
                {
                    return;
                }

                if (workContext.CurrentUser == null)
                {
                    return;
                }

                var membershipService = EngineContext.Current.Resolve<IMembershipService>();
                AsyncHelper.RunSync(() => membershipService.SaveProfileEntry(workContext.CurrentUser.Id, ThemeUserProfileProvider.Fields.PreferredTheme, value));

                //clear cache
                this.isDesktopThemeCached = false;
            }
        }
    }
}