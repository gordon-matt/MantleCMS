using Mantle.Localization;

namespace Mantle.Plugins.Widgets.Bootstrap3.Infrastructure
{
    public class LanguagePackInvariant : ILanguagePack
    {
        #region ILanguagePack Members

        public string CultureCode => null;

        public IDictionary<string, string> LocalizedStrings => new Dictionary<string, string>
        {
            { LocalizableStrings.ContentBlocks.CarouselBlock.Interval, "Interval" },
            { LocalizableStrings.ContentBlocks.CarouselBlock.Keyboard, "React to Keyboard Events" },
            { LocalizableStrings.ContentBlocks.CarouselBlock.MediaFolder, "Media Folder" },
            { LocalizableStrings.ContentBlocks.CarouselBlock.OnSlideEvent, "On Slide Event" },
            { LocalizableStrings.ContentBlocks.CarouselBlock.OnSlidEvent, "On Slid Event" },
            { LocalizableStrings.ContentBlocks.CarouselBlock.PauseOnHover, "Pause on Hover" },
            { LocalizableStrings.ContentBlocks.CarouselBlock.StartIndex, "Start Index" },
            { LocalizableStrings.ContentBlocks.CarouselBlock.Wrap, "Cycle Continuously" },
            { LocalizableStrings.ContentBlocks.ImageGalleryBlock.ImagesPerRowL, "# Images Per Row (L)" },
            { LocalizableStrings.ContentBlocks.ImageGalleryBlock.ImagesPerRowM, "# Images Per Row (M)" },
            { LocalizableStrings.ContentBlocks.ImageGalleryBlock.ImagesPerRowS, "# Images Per Row (S)" },
            { LocalizableStrings.ContentBlocks.ImageGalleryBlock.ImagesPerRowXS, "# Images Per Row (XS)" },
            { LocalizableStrings.ContentBlocks.ImageGalleryBlock.MediaFolder, "Media Folder" },
        };

        #endregion ILanguagePack Members
    }
}