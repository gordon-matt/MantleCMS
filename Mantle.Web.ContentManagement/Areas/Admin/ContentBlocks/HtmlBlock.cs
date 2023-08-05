namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

public class HtmlBlock : ContentBlockBase
{
    #region IContentBlock Members

    public override string Name => "Html Content Block";

    public override string DisplayTemplatePath => "/Areas/Admin/ContentBlocks/Views/Shared/DisplayTemplates/HtmlBlock.cshtml";

    public override string EditorTemplatePath => "/Areas/Admin/ContentBlocks/Views/Shared/EditorTemplates/HtmlBlock.cshtml";

    #endregion IContentBlock Members

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.HtmlBlock.BodyContent)]
    [LocalizedHelpText(MantleCmsLocalizableStrings.ContentBlocks.HtmlBlock.HelpText.BodyContent)]
    public string BodyContent { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.HtmlBlock.Script)]
    [LocalizedHelpText(MantleCmsLocalizableStrings.ContentBlocks.HtmlBlock.HelpText.Script)]
    public string Script { get; set; }
}