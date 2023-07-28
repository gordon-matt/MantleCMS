namespace Mantle.Web.ContentManagement.Areas.Admin.Pages;

public class PageSettings : ISettings
{
    public PageSettings()
    {
        NumberOfPageVersionsToKeep = 5;
    }

    #region ISettings Members

    public string Name => "CMS: Page Settings";

    public bool IsTenantRestricted => false;

    public string EditorTemplatePath => "Mantle.Web.ContentManagement.Areas.Admin.Pages.Views.Shared.EditorTemplates.PageSettings.cshtml";

    #endregion ISettings Members

    [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Pages.NumberOfPageVersionsToKeep)]
    public short NumberOfPageVersionsToKeep { get; set; }
}