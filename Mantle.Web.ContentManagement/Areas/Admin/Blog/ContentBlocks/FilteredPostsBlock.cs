using Mantle.ComponentModel;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.ContentBlocks
{
    public class FilteredPostsBlock : ContentBlockBase
    {
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.AllPostsBlock.CategoryId)]
        public int? CategoryId { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.AllPostsBlock.TagId)]
        public int? TagId { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.AllPostsBlock.FilterType)]
        public FilterType FilterType { get; set; }

        #region ContentBlockBase Overrides

        public override string Name
        {
            get { return "Blog: Filtered Posts"; }
        }

        public override string DisplayTemplatePath
        {
            get { return "Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.Shared.DisplayTemplates.FilteredPostsBlock"; }
        }

        public override string EditorTemplatePath
        {
            get { return "Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.Shared.EditorTemplates.FilteredPostsBlock"; }
        }

        #endregion ContentBlockBase Overrides
    }

    public enum FilterType : byte
    {
        None = 0,
        Category = 1,
        Tag = 2
    }
}