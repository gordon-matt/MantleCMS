using Mantle.ComponentModel;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.ContentBlocks
{
    public class CategoriesBlock : ContentBlockBase
    {
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.CategoriesBlock.NumberOfCategories)]
        public byte NumberOfCategories { get; set; }

        #region ContentBlockBase Overrides

        public override string Name => "Blog: Categories";

        public override string DisplayTemplatePath => "Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.Shared.DisplayTemplates.CategoriesBlock.cshtml";

        public override string EditorTemplatePath => "Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.Shared.EditorTemplates.CategoriesBlock.cshtml";

        #endregion ContentBlockBase Overrides
    }
}