using Mantle.ComponentModel;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks
{
    public class HtmlBlock : ContentBlockBase
    {
        #region IContentBlock Members

        public override string Name
        {
            get { return "Html Content Block"; }
        }

        public override string DisplayTemplatePath
        {
            get { return "Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Views.Shared.DisplayTemplates.HtmlBlock"; }
        }

        public override string EditorTemplatePath
        {
            get { return "Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Views.Shared.EditorTemplates.HtmlBlock"; }
        }

        #endregion IContentBlock Members

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.HtmlBlock.BodyContent)]
        [LocalizedHelpText(MantleCmsLocalizableStrings.ContentBlocks.HtmlBlock.HelpText.BodyContent)]
        public string BodyContent { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.HtmlBlock.Script)]
        [LocalizedHelpText(MantleCmsLocalizableStrings.ContentBlocks.HtmlBlock.HelpText.Script)]
        public string Script { get; set; }
    }
}