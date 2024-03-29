﻿using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Web.ContentManagement.Areas.Admin.Localization;

public class LanguageSwitchBlock : ContentBlockBase
{
    public enum LanguageSwitchStyle : byte
    {
        BootstrapNavbarDropdown = 0,
        Select = 1,
        List = 2
    }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.LanguageSwitchBlock.Style)]
    public LanguageSwitchStyle Style { get; set; }

    //[BlockProperty]
    //[LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.LanguageSwitchBlock.UseUrlPrefix)]
    //public bool UseUrlPrefix { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.LanguageSwitchBlock.IncludeInvariant)]
    public bool IncludeInvariant { get; set; }

    [BlockProperty("[ Invariant ]")]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.LanguageSwitchBlock.InvariantText)]
    public string InvariantText { get; set; }

    #region ContentBlockBase Overrides

    public override string Name => "Language Switch";

    public override string DisplayTemplatePath => "/Areas/Admin/Localization/Views/Shared/DisplayTemplates/LanguageSwitchBlock.cshtml";

    public override string EditorTemplatePath => "/Areas/Admin/Localization/Views/Shared/EditorTemplates/LanguageSwitchBlock.cshtml";

    #endregion ContentBlockBase Overrides
}