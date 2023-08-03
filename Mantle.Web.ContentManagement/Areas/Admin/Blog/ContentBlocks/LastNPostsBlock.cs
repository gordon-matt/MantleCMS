using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.ContentBlocks;

public class LastNPostsBlock : ContentBlockBase
{
    [BlockProperty(5)]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.LastNPostsBlock.NumberOfEntries)]
    public byte NumberOfEntries { get; set; }

    #region ContentBlockBase Overrides

    public override string Name => "Blog: Last (N) Posts";

    public override string DisplayTemplatePath => "Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.Shared.DisplayTemplates.LastNPostsBlock.cshtml";

    public override string EditorTemplatePath => "Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.Shared.EditorTemplates.LastNPostsBlock.cshtml";

    #endregion ContentBlockBase Overrides
}