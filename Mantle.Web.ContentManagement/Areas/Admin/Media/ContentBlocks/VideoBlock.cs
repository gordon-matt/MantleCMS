using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Web.ContentManagement.Areas.Admin.Media.ContentBlocks;

public class VideoBlock : ContentBlockBase
{
    public enum VideoType : byte
    {
        Normal = 0,
        Flash = 1,
        Silverlight = 2
    }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.ControlId)]
    public string ControlId { get; set; }

    [BlockProperty(VideoType.Normal)]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.Type)]
    public VideoType Type { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.Source)]
    public string Source { get; set; }

    [BlockProperty(true)]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.ShowControls)]
    public bool ShowControls { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.AutoPlay)]
    public bool AutoPlay { get; set; }

    [BlockProperty]
    [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.Loop)]
    public bool Loop { get; set; }

    #region IContentBlock Members

    public override string Name => "Video Block";

    public override string DisplayTemplatePath => "Mantle.Web.ContentManagement.Areas.Admin.Media.Views.Shared.DisplayTemplates.VideoBlock.cshtml";

    public override string EditorTemplatePath => "Mantle.Web.ContentManagement.Areas.Admin.Media.Views.Shared.EditorTemplates.VideoBlock.cshtml";

    #endregion IContentBlock Members
}