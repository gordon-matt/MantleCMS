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

        [BlockProperty]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.ImageGalleryBlock.MediaFolder)]
        public string MediaFolder { get; set; }

        [BlockProperty(ImagesPerRow.Two)]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.ImageGalleryBlock.ImagesPerRowXS)]
        public ImagesPerRow ImagesPerRowXS { get; set; }

        [BlockProperty(ImagesPerRow.Three)]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.ImageGalleryBlock.ImagesPerRowS)]
        public ImagesPerRow ImagesPerRowS { get; set; }

        [BlockProperty(ImagesPerRow.Three)]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.ImageGalleryBlock.ImagesPerRowM)]
        public ImagesPerRow ImagesPerRowM { get; set; }

        [BlockProperty(ImagesPerRow.Four)]
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

        public static string GetThumbSizeCss(ImagesPerRow imagesPerRow, string size = "md")
        {
            switch (imagesPerRow)
            {
                case ImagesPerRow.Two: return string.Concat("col-", size, "-6");
                case ImagesPerRow.Three: return string.Concat("col-", size, "-4");
                case ImagesPerRow.Four: return string.Concat("col-", size, "-3");
                case ImagesPerRow.Six: return string.Concat("col-", size, "-2");
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

        public enum ImagesPerRow
        {
            Two = 2,
            Three = 3,
            Four = 4,
            Six = 6
        }

        protected override string RenderObservableDeclaration(BlockPropertyInfo property)
        {
            if (property.Name == nameof(MediaFolder))
            {
                return null;
            }
            return base.RenderObservableDeclaration(property);
        }

        protected override string RenderObservableAssignment(BlockPropertyInfo property)
        {
            if (property.Name == nameof(MediaFolder))
            {
                // If MediaFolder is not set, it means the contentBlock is new and we have nothing in blockValues
                return
$@"		if (data.{property.Name} == undefined) {{ return; }}
		$('#{property.Name}').val(data.{property.Name});";
            }

            return base.RenderObservableAssignment(property);
        }

        protected override string RenderSaveValue(BlockPropertyInfo property, bool isLast)
        {
            if (property.Name == nameof(MediaFolder))
            {
                return $"\t\t\t{property.Name}: $('#{property.Name}').val(),";
            }

            return base.RenderSaveValue(property, isLast);
        }
    }
}