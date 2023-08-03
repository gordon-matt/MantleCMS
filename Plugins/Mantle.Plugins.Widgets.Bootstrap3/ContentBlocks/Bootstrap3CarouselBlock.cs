using Mantle.Localization.ComponentModel;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Plugins.Widgets.Bootstrap3.ContentBlocks
{
    public class Bootstrap3CarouselBlock : ContentBlockBase
    {
        public Bootstrap3CarouselBlock()
        {
            Interval = 5000;
            Keyboard = true;
            PauseOnHover = true;
            Wrap = true;
        }

        [BlockProperty]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.CarouselBlock.MediaFolder)]
        public string MediaFolder { get; set; }

        /// <summary>
        /// The amount of time to delay between automatically cycling an item. If false, carousel will not automatically cycle.
        /// </summary>
        [BlockProperty(5000)]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.CarouselBlock.Interval)]
        public int Interval { get; set; }

        /// <summary>
        /// Pass a raw slide index to the carousel
        /// </summary>
        [BlockProperty]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.CarouselBlock.StartIndex)]
        public byte StartIndex { get; set; }

        /// <summary>
        /// Whether the carousel should react to keyboard events.
        /// </summary>
        [BlockProperty(true)]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.CarouselBlock.Keyboard)]
        public bool Keyboard { get; set; }

        /// <summary>
        /// Pauses the cycling of the carousel on mouseenter and resumes the cycling of the carousel on mouseleave.
        /// </summary>
        [BlockProperty(true)]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.CarouselBlock.PauseOnHover)]
        public bool PauseOnHover { get; set; }

        /// <summary>
        /// Whether the carousel should cycle continuously or have hard stops.
        /// </summary>
        [BlockProperty(true)]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.CarouselBlock.Wrap)]
        public bool Wrap { get; set; }

        /// <summary>
        /// This event fires immediately when the slide instance method is invoked.
        /// </summary>
        [BlockProperty]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.CarouselBlock.OnSlideEvent)]
        public string OnSlideEvent { get; set; }

        /// <summary>
        /// This event is fired when the carousel has completed its slide transition.
        /// </summary>
        [BlockProperty]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.CarouselBlock.OnSlidEvent)]
        public string OnSlidEvent { get; set; }

        #region ContentBlockBase Overrides

        public override string Name
        {
            get { return "Bootstrap 3: Carousel"; }
        }

        public override string DisplayTemplatePath
        {
            get { return "/Plugins/Widgets.Bootstrap3/Views/Shared/DisplayTemplates/CarouselBlock.cshtml"; }
        }

        public override string EditorTemplatePath
        {
            get { return "/Plugins/Widgets.Bootstrap3/Views/Shared/EditorTemplates/CarouselBlock.cshtml"; }
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

        #endregion ContentBlockBase Overrides
    }
}