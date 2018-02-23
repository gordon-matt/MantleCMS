using Mantle.ComponentModel;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Web.ContentManagement.Areas.Admin.Localization
{
    public class LanguageSwitchBlock : ContentBlockBase
    {
        public enum LanguageSwitchStyle : byte
        {
            BootstrapNavbarDropdown = 0,
            Select = 1,
            List = 2
        }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.LanguageSwitchBlock.Style)]
        public LanguageSwitchStyle Style { get; set; }

        //[LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.LanguageSwitchBlock.UseUrlPrefix)]
        //public bool UseUrlPrefix { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.LanguageSwitchBlock.IncludeInvariant)]
        public bool IncludeInvariant { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.LanguageSwitchBlock.InvariantText)]
        public string InvariantText { get; set; }

        #region ContentBlockBase Overrides

        public override string Name => "Language Switch";

        public override string DisplayTemplatePath => "Mantle.Web.ContentManagement.Areas.Admin.Localization.Views.Shared.DisplayTemplates.LanguageSwitchBlock.cshtml";

        public override string EditorTemplatePath => "Mantle.Web.ContentManagement.Areas.Admin.Localization.Views.Shared.EditorTemplates.LanguageSwitchBlock.cshtml";

        #endregion ContentBlockBase Overrides
    }
}