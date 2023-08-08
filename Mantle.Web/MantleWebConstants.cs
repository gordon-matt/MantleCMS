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

    public static class AdminCss
    {
        public static class Columns
        {
            public const string Full = "col-xs-12 col-sm-12 col-md-12 col-lg-12 col-xl-12 col-xxl-12 admin-column";
            public const string Half = "col-xs-12 col-sm-6 col-md-6 col-lg-6 col-xl-6 col-xxl-6 admin-column";
            public const string HalfStrict = "col-xs-6 col-sm-6 col-md-6 col-lg-6 col-xl-6 col-xxl-6 admin-column";
            public const string Third = "col-xs-12 col-sm-4 col-md-4 col-lg-4 col-xl-4 col-xxl-4 admin-column";
            public const string ThirdStrict = "col-xs-4 col-sm-4 col-md-4 col-lg-4 col-xl-4 col-xxl-4 admin-column";
            public const string Quarter = "col-xs-12 col-sm-3 col-md-3 col-lg-3 col-xl-3 col-xxl-3 admin-column";
            public const string QuarterStrict = "col-xs-3 col-sm-3 col-md-3 col-lg-3 col-xl-3 col-xxl-3 admin-column";
            public const string ThreeQuarters = "col-xs-12 col-sm-9 col-md-9 col-lg-9 col-xl-9 col-xxl-9 admin-column";
            public const string ThreeQuartersStrict = "col-xs-9 col-sm-9 col-md-9 col-lg-9 col-xl-9 col-xxl-9 admin-column";
            public const string Sixth = "col-xs-12 col-sm-2 col-md-2 col-lg-2 col-xl-2 col-xxl-2 admin-column";
            public const string SixthStrict = "col-xs-2 col-sm-2 col-md-2 col-lg-2 col-xl-2 col-xxl-2 admin-column";
            public const string FiveSixths = "col-xs-12 col-sm-10 col-md-10 col-lg-10 col-xl-10 col-xxl-10 admin-column";
            public const string FiveSixthsStrict = "col-xs-10 col-sm-10 col-md-10 col-lg-10 col-xl-10 col-xxl-10 admin-column";
        }
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