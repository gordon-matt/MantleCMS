using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.ContentBlocks;

public class FilteredPostsBlock : ContentBlockBase
{
    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.AllPostsBlock.CategoryId)]
    public int? CategoryId { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.AllPostsBlock.TagId)]
    public int? TagId { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.AllPostsBlock.FilterType)]
    public FilterType FilterType { get; set; }

    #region ContentBlockBase Overrides

    public override string Name => "Blog: Filtered Posts";

    public override string DisplayTemplatePath => "/Areas/Admin/Blog/Views/Shared/DisplayTemplates/FilteredPostsBlock.cshtml";

    public override string EditorTemplatePath => "/Areas/Admin/Blog/Views/Shared/EditorTemplates/FilteredPostsBlock.cshtml";

    #endregion ContentBlockBase Overrides
}

public enum FilterType : byte
{
    None = 0,
    Category = 1,
    Tag = 2
}