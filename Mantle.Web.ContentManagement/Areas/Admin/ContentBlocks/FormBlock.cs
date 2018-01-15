using Mantle.ComponentModel;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks
{
    public class FormBlock : ContentBlockBase
    {
        //[AllowHtml]
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.HtmlTemplate)]
        public string HtmlTemplate { get; set; }

        //[AllowHtml]
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.ThankYouMessage)]
        public string ThankYouMessage { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.RedirectUrl)]
        public string RedirectUrl { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.EmailAddress)]
        [LocalizedHelpText(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.HelpText.EmailAddress)]
        public string EmailAddress { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.UseAjax)]
        public bool UseAjax { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.FormUrl)]
        [LocalizedHelpText(MantleCmsLocalizableStrings.ContentBlocks.FormBlock.HelpText.FormUrl)]
        public string FormUrl { get; set; }

        #region ContentBlockBase Overrides

        public override string Name
        {
            get { return "Form Content Block"; }
        }

        public override string DisplayTemplatePath
        {
            get { return "Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Views.Shared.DisplayTemplates.FormBlock"; }
        }

        public override string EditorTemplatePath
        {
            get { return "Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Views.Shared.EditorTemplates.FormBlock"; }
        }

        #endregion ContentBlockBase Overrides
    }
}