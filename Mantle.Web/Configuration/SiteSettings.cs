namespace Mantle.Web.Configuration;

public class SiteSettings : BaseResourceSettings
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

    public override string Name => "Site Settings";

    public override string EditorTemplatePath => "/Views/Shared/EditorTemplates/SiteSettings.cshtml";

    #endregion ISettings Members

    #region IResourceSettings Members

    public override ICollection<RequiredResourceCollection> Resources { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public override ICollection<RequiredResourceCollection> DefaultResources => new List<RequiredResourceCollection>
    {
        new RequiredResourceCollection
        {
            Name = "Bootstrap-FileInput",
            Resources = new List<RequiredResource>
            {
                new RequiredResource { Type = ResourceType.Script, Order = 0, Path = "https://cdn.jsdelivr.net/npm/bootstrap-fileinput@5.5.2/js/fileinput.min.js" },
                new RequiredResource { Type = ResourceType.Stylesheet, Order = 0, Path = "https://cdn.jsdelivr.net/npm/bootstrap-fileinput@5.5.2/css/fileinput.min.css" }
            }
        }
    };

    #endregion IResourceSettings Members

    #region General

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.SiteName)]
    [SettingsProperty("My Site")]
    public string SiteName { get; set; }

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.DefaultFrontendLayoutPath)]
    [SettingsProperty("~/Views/Shared/_Layout.cshtml")]
    public string DefaultFrontendLayoutPath { get; set; }

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.AdminLayoutPath)]
    [SettingsProperty("~/Areas/Admin/Views/Shared/_Layout.cshtml")]
    public string AdminLayoutPath { get; set; }

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.DefaultGridPageSize)]
    [SettingsProperty(10)]
    public int DefaultGridPageSize { get; set; }

    #endregion General

    #region Themes

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.DefaultTheme)]
    [SettingsProperty("Default")]
    public string DefaultTheme { get; set; }

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.AllowUserToSelectTheme)]
    [SettingsProperty]
    public bool AllowUserToSelectTheme { get; set; }

    #endregion Themes

    #region Localization

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.DefaultLanguage)]
    [SettingsProperty]
    public string DefaultLanguage { get; set; }

    #endregion Localization

    #region SEO

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.DefaultMetaKeywords)]
    [SettingsProperty]
    public string DefaultMetaKeywords { get; set; }

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.DefaultMetaDescription)]
    [SettingsProperty]
    public string DefaultMetaDescription { get; set; }

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Site.HomePageTitle)]
    [SettingsProperty("Home Page")]
    public string HomePageTitle { get; set; }

    #endregion SEO
}