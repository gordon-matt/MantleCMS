using Mantle.Localization.ComponentModel;

namespace Mantle.Web.Configuration
{
    public class SiteSettings : ISettings
    {
        public SiteSettings()
        {
            SiteName = "My Site";
            DefaultTheme = "Default";
            DefaultGridPageSize = 10;
            DefaultFrontendLayoutPath = "~/Views/Shared/_Layout.cshtml";
            AdminLayoutPath = "~/Areas/Admin/Views/Shared/_Layout.cshtml";
            HomePageTitle = "Home Page";
        }

        #region ISettings Members

        public string Name => "Site Settings";

        public bool IsTenantRestricted => false;

        public string EditorTemplatePath => "Mantle.Web.Views.Shared.EditorTemplates.SiteSettings.cshtml";

        #endregion ISettings Members

        #region General

        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.SiteName)]
        public string SiteName { get; set; }

        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.DefaultFrontendLayoutPath)]
        public string DefaultFrontendLayoutPath { get; set; }

        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.AdminLayoutPath)]
        public string AdminLayoutPath { get; set; }

        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.DefaultGridPageSize)]
        public int DefaultGridPageSize { get; set; }

        #endregion General

        #region Themes

        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.DefaultTheme)]
        public string DefaultTheme { get; set; }

        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.AllowUserToSelectTheme)]
        public bool AllowUserToSelectTheme { get; set; }

        #endregion Themes

        #region Localization

        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.DefaultLanguage)]
        public string DefaultLanguage { get; set; }

        #endregion Localization

        #region SEO

        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.DefaultMetaKeywords)]
        public string DefaultMetaKeywords { get; set; }

        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.DefaultMetaDescription)]
        public string DefaultMetaDescription { get; set; }

        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.HomePageTitle)]
        public string HomePageTitle { get; set; }

        #endregion SEO
    }
}