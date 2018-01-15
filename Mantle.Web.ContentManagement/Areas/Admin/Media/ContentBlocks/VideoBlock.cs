using Mantle.ComponentModel;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Web.ContentManagement.Areas.Admin.Media.ContentBlocks
{
    public class VideoBlock : ContentBlockBase
    {
        public enum VideoType : byte
        {
            Normal = 0,
            Flash = 1,
            Silverlight = 2
        }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.ControlId)]
        public string ControlId { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.Type)]
        public VideoType Type { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.Source)]
        public string Source { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.ShowControls)]
        public bool ShowControls { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.AutoPlay)]
        public bool AutoPlay { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.Loop)]
        public bool Loop { get; set; }

        #region IContentBlock Members

        public override string Name
        {
            get { return "Video Block"; }
        }

        public override string DisplayTemplatePath
        {
            get { return "Mantle.Web.ContentManagement.Areas.Admin.Media.Views.Shared.DisplayTemplates.VideoBlock"; }
        }

        public override string EditorTemplatePath
        {
            get { return "Mantle.Web.ContentManagement.Areas.Admin.Media.Views.Shared.EditorTemplates.VideoBlock"; }
        }

        #endregion IContentBlock Members
    }
}