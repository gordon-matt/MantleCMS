using Mantle.Localization.ComponentModel;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Plugins.Widgets.Bootstrap3.ContentBlocks
{
    public class Bootstrap3ImageGalleryBlock : ContentBlockBase
    {
        public Bootstrap3ImageGalleryBlock()
        {
            ImagesPerRowXS = ImagesPerRow.Two;
            ImagesPerRowS = ImagesPerRow.Three;
            ImagesPerRowM = ImagesPerRow.Three;
            ImagesPerRowL = ImagesPerRow.Four;
        }

        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.ImageGalleryBlock.MediaFolder)]
        public string MediaFolder { get; set; }

        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.ImageGalleryBlock.ImagesPerRowXS)]
        public ImagesPerRow ImagesPerRowXS { get; set; }

        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.ImageGalleryBlock.ImagesPerRowS)]
        public ImagesPerRow ImagesPerRowS { get; set; }

        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.ImageGalleryBlock.ImagesPerRowM)]
        public ImagesPerRow ImagesPerRowM { get; set; }

        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.ImageGalleryBlock.ImagesPerRowL)]
        public ImagesPerRow ImagesPerRowL { get; set; }

        #region ContentBlockBase Overrides

        public override string DisplayTemplatePath
        {
            get { return "/Plugins/Widgets.Bootstrap3/Views/Shared/DisplayTemplates/ImageGalleryBlock.cshtml"; }
        }

        public override string EditorTemplatePath
        {
            get { return "/Plugins/Widgets.Bootstrap3/Views/Shared/EditorTemplates/ImageGalleryBlock.cshtml"; }
        }

        public override string Name
        {
            get { return "Bootstrap 3: Image Gallery"; }
        }

        #endregion ContentBlockBase Overrides

        public enum ImagesPerRow
        {
            Two = 2,
            Three = 3,
            Four = 4,
            Six = 6
        }

        public static string GetThumbSizeCss(ImagesPerRow imagesPerRow, string size = "md")
        {
            switch (imagesPerRow)
            {
                case Bootstrap3ImageGalleryBlock.ImagesPerRow.Two: return string.Concat("col-", size, "-6");
                case Bootstrap3ImageGalleryBlock.ImagesPerRow.Three: return string.Concat("col-", size, "-4");
                case Bootstrap3ImageGalleryBlock.ImagesPerRow.Four: return string.Concat("col-", size, "-3");
                case Bootstrap3ImageGalleryBlock.ImagesPerRow.Six: return string.Concat("col-", size, "-2");
                default:
                    {
                        return size switch
                        {
                            "xs" => "col-xs-6",
                            "lg" => "col-lg-3",
                            _ => "col-md-4",
                        };
                    }
            }
        }
    }
}