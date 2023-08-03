namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

public class FormBlock : ContentBlockBase
{
    //[AllowHtml]
    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.HtmlTemplate)]
    public string HtmlTemplate { get; set; }

    //[AllowHtml]
    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.ThankYouMessage)]
    public string ThankYouMessage { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.RedirectUrl)]
    public string RedirectUrl { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.EmailAddress)]
    [LocalizedHelpText(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.HelpText.EmailAddress)]
    public string EmailAddress { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.UseAjax)]
    public bool UseAjax { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.FormUrl)]
    [LocalizedHelpText(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.HelpText.FormUrl)]
    public string FormUrl { get; set; }

    #region ContentBlockBase Overrides

    public override string Name => "Form Content Block";

    public override string DisplayTemplatePath => "Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Views.Shared.DisplayTemplates.FormBlock.cshtml";

    public override string EditorTemplatePath => "Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Views.Shared.EditorTemplates.FormBlock.cshtml";

    #endregion ContentBlockBase Overrides
}