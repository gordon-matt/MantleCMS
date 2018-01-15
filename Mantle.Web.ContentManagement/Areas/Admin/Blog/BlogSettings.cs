using Mantle.ComponentModel;
using Mantle.Web.Configuration;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog
{
    public class BlogSettings : ISettings
    {
        public BlogSettings()
        {
            DateFormat = "YYYY-MM-DD HH:mm:ss";
            ItemsPerPage = 5;
            PageTitle = "Blog";
            ShowOnMenus = true;
            MenuPosition = 0;
        }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.PageTitle)]
        public string PageTitle { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.DateFormat)]
        public string DateFormat { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.ItemsPerPage)]
        public byte ItemsPerPage { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.ShowOnMenus)]
        public bool ShowOnMenus { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.MenuPosition)]
        public byte MenuPosition { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.UseAjax)]
        public bool UseAjax { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.AccessRestrictions)]
        public string AccessRestrictions { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.LayoutPathOverride)]
        public string LayoutPathOverride { get; set; }

        #region ISettings Members

        public string Name
        {
            get { return "CMS: Blog Settings"; }
        }

        public bool IsTenantRestricted
        {
            get { return false; }
        }

        public string EditorTemplatePath
        {
            get { return "Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.Shared.EditorTemplates.BlogSettings"; }
        }

        #endregion ISettings Members
    }
}