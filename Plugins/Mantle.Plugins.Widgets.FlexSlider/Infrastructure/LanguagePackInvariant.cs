using Mantle.Localization;

namespace Mantle.Plugins.Widgets.FlexSlider.Infrastructure
{
    public class LanguagePackInvariant : ILanguagePack
    {
        #region ILanguagePack Members

        public string CultureCode => null;

        public IDictionary<string, string> LocalizedStrings => new Dictionary<string, string>
        {
            { LocalizableStrings.FlexSliderBlock.Animation, "Animation" },
            { LocalizableStrings.FlexSliderBlock.AnimationLoop, "Animation Loop" },
            { LocalizableStrings.FlexSliderBlock.AnimationSpeed, "Animation Speed" },
            { LocalizableStrings.FlexSliderBlock.AsNavFor, "As Nav For" },
            { LocalizableStrings.FlexSliderBlock.ControlId, "Control ID" },
            { LocalizableStrings.FlexSliderBlock.ControlNav, "Control Nav" },
            { LocalizableStrings.FlexSliderBlock.ControlsContainer, "Controls Container" },
            { LocalizableStrings.FlexSliderBlock.Direction, "Direction" },
            { LocalizableStrings.FlexSliderBlock.DirectionNav, "Direction Nav" },
            { LocalizableStrings.FlexSliderBlock.Easing, "Easing" },
            { LocalizableStrings.FlexSliderBlock.InitDelay, "Initialization Delay" },
            { LocalizableStrings.FlexSliderBlock.ItemMargin, "Item Margin" },
            { LocalizableStrings.FlexSliderBlock.ItemWidth, "Item Width" },
            { LocalizableStrings.FlexSliderBlock.Keyboard, "Keyboard" },
            { LocalizableStrings.FlexSliderBlock.ManualControls, "Manual Controls" },
            { LocalizableStrings.FlexSliderBlock.MaxItems, "Max Items" },
            { LocalizableStrings.FlexSliderBlock.MediaFolder, "Media Folder" },
            { LocalizableStrings.FlexSliderBlock.MinItems, "Min Items" },
            { LocalizableStrings.FlexSliderBlock.Mousewheel, "Mousewheel" },
            { LocalizableStrings.FlexSliderBlock.Move, "Move" },
            { LocalizableStrings.FlexSliderBlock.MultipleKeyboard, "Multiple Keyboard" },
            { LocalizableStrings.FlexSliderBlock.Namespace, "Namespace" },
            { LocalizableStrings.FlexSliderBlock.NextText, "Next Text" },
            { LocalizableStrings.FlexSliderBlock.OnAdded, "OnAdded" },
            { LocalizableStrings.FlexSliderBlock.OnAfter, "OnAfter" },
            { LocalizableStrings.FlexSliderBlock.OnBefore, "OnBefore" },
            { LocalizableStrings.FlexSliderBlock.OnEnd, "OnEnd" },
            { LocalizableStrings.FlexSliderBlock.OnRemoved, "OnRemoved" },
            { LocalizableStrings.FlexSliderBlock.OnStart, "OnStart" },
            { LocalizableStrings.FlexSliderBlock.PauseOnAction, "Pause on Action" },
            { LocalizableStrings.FlexSliderBlock.PauseOnHover, "Pause on Hover" },
            { LocalizableStrings.FlexSliderBlock.PausePlay, "Pause/Play" },
            { LocalizableStrings.FlexSliderBlock.PauseText, "Pause Text" },
            { LocalizableStrings.FlexSliderBlock.PlayText, "Play Text" },
            { LocalizableStrings.FlexSliderBlock.PrevText, "Previous Text" },
            { LocalizableStrings.FlexSliderBlock.Randomize, "Randomize" },
            { LocalizableStrings.FlexSliderBlock.Reverse, "Reverse" },
            { LocalizableStrings.FlexSliderBlock.Selector, "Selector" },
            { LocalizableStrings.FlexSliderBlock.Slideshow, "Slideshow" },
            { LocalizableStrings.FlexSliderBlock.SlideshowSpeed, "Slideshow Speed" },
            { LocalizableStrings.FlexSliderBlock.SmoothHeight, "Smooth Height" },
            { LocalizableStrings.FlexSliderBlock.StartAt, "Start At" },
            { LocalizableStrings.FlexSliderBlock.Sync, "Sync" },
            { LocalizableStrings.FlexSliderBlock.Touch, "Touch" },
            { LocalizableStrings.FlexSliderBlock.UseCSS, "Use CSS3 Transitions" },
            { LocalizableStrings.FlexSliderBlock.Video, "Video" },
            { LocalizableStrings.FlexSliderBlock.EditorTabs.Animation, "Animation" },
            { LocalizableStrings.FlexSliderBlock.EditorTabs.Events, "Events (Advanced)" },
            { LocalizableStrings.FlexSliderBlock.EditorTabs.General, "General" },
            { LocalizableStrings.FlexSliderBlock.EditorTabs.Navigation, "Navigation" },
            { LocalizableStrings.FlexSlider, "FlexSlider" },
            { LocalizableStrings.CategoryGallerySliders, "Category Gallery Sliders" },
            { LocalizableStrings.CollectionGallerySliders, "Collection Gallery Sliders" },
            { LocalizableStrings.DefaultSliders, "Default Sliders" },
            { LocalizableStrings.LinkedSliders, "Linked Sliders" }
        };

        #endregion ILanguagePack Members
    }
}