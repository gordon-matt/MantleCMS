using Mantle.ComponentModel;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.ContentBlocks
{
    public class LastNPostsBlock : ContentBlockBase
    {
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.LastNPostsBlock.NumberOfEntries)]
        public byte NumberOfEntries { get; set; }

        #region ContentBlockBase Overrides

        public override string Name
        {
            get { return "Blog: Last (N) Posts"; }
        }

        public override string DisplayTemplatePath
        {
            get { return "Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.Shared.DisplayTemplates.LastNPostsBlock"; }
        }

        public override string EditorTemplatePath
        {
            get { return "Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.Shared.EditorTemplates.LastNPostsBlock"; }
        }

        #endregion ContentBlockBase Overrides
    }
}