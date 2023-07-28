namespace Mantle.Web;

public class MantleWebConstants
{
    private static string defaultAdminLayoutPath;
    private static string defaultFrontendLayoutPath;

    public static string DefaultAdminLayoutPath
    {
        get
        {
            if (string.IsNullOrEmpty(defaultAdminLayoutPath))
            {
                var siteSettings = EngineContext.Current.Resolve<SiteSettings>();

                defaultAdminLayoutPath = string.IsNullOrEmpty(siteSettings.AdminLayoutPath)
                    ? "~/Areas/Admin/Views/Shared/_Layout.cshtml"
                    : siteSettings.AdminLayoutPath;
            }
            return defaultAdminLayoutPath;
        }
    }

    public static string DefaultFrontendLayoutPath
    {
        get
        {
            if (string.IsNullOrEmpty(defaultFrontendLayoutPath))
            {
                var siteSettings = EngineContext.Current.Resolve<SiteSettings>();

                defaultFrontendLayoutPath = string.IsNullOrEmpty(siteSettings.DefaultFrontendLayoutPath)
                    ? "~/Views/Shared/_Layout.cshtml"
                    : siteSettings.DefaultFrontendLayoutPath;
            }
            return defaultFrontendLayoutPath;
        }
    }

    public static class Areas
    {
        public const string Admin = "Admin";
        public const string Configuration = "Admin/Configuration";

        //public const string Indexing = "Admin/Indexing";
        public const string Localization = "Admin/Localization";

        public const string Log = "Admin/Log";
        public const string Membership = "Admin/Membership";
        public const string Plugins = "Admin/Plugins";
        public const string ScheduledTasks = "Admin/ScheduledTasks";
        public const string Tenants = "Admin/Tenants";
    }

    public static class CacheKeys
    {
        public const string CurrentCulture = "Mantle.Web.CacheKeys.CurrentCulture";

        /// <summary>
        /// {0} for TenantId, {1} for settings "Type"
        /// </summary>
        public const string SettingsKeyFormat = "Mantle.Web.CacheKeys.Settings.Tenant_{0}_{1}";

        /// <summary>
        /// {0}: Tenant ID
        /// </summary>
        public const string SettingsKeysPatternFormat = "Mantle.Web.CacheKeys.Settings.Tenant_{0}_.*";
    }

    public static class StateProviders
    {
        public const string CurrentCultureCode = "CurrentCultureCode";
        public const string CurrentTheme = "CurrentTheme";
        public const string CurrentUser = "CurrentUser";
    }

    /// <summary>
    /// Resets static variables to NULL
    /// </summary>
    public static void ResetCache()
    {
        defaultAdminLayoutPath = null;
        defaultFrontendLayoutPath = null;
    }
}