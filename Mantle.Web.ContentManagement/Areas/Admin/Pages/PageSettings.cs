using Mantle.ComponentModel;
using Mantle.Web.Configuration;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages
{
    public class PageSettings : ISettings
    {
        public PageSettings()
        {
            NumberOfPageVersionsToKeep = 5;
        }

        #region ISettings Members

        public string Name
        {
            get { return "CMS: Page Settings"; }
        }

        public bool IsTenantRestricted
        {
            get { return false; }
        }

        public string EditorTemplatePath
        {
            get { return "Mantle.Web.ContentManagement.Areas.Admin.Pages.Views.Shared.EditorTemplates.PageSettings"; }
        }

        #endregion ISettings Members

        [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Pages.NumberOfPageVersionsToKeep)]
        public short NumberOfPageVersionsToKeep { get; set; }
    }
}