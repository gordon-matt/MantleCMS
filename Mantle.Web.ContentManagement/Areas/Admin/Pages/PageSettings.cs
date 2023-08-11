using Mantle.Web.Configuration;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages;

public class PageSettings : BaseSettings
{
    public PageSettings()
    {
        NumberOfPageVersionsToKeep = 5;
    }

    #region ISettings Members

    public override string Name => "CMS: Page Settings";

    public override string EditorTemplatePath => "/Areas/Admin/Pages/Views/Shared/EditorTemplates/PageSettings.cshtml";

    #endregion ISettings Members

    [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Pages.NumberOfPageVersionsToKeep)]
    [SettingsProperty(5)]
    public short NumberOfPageVersionsToKeep { get; set; }
}