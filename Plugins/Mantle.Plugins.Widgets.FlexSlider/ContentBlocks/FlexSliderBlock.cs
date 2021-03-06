﻿using System.Text;
using Mantle.Localization.ComponentModel;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Microsoft.AspNetCore.Html;

namespace Mantle.Plugins.Widgets.FlexSlider.ContentBlocks
{
    public class FlexSliderBlock : ContentBlockBase
    {
        public FlexSliderBlock()
        {
            Animation = AnimationType.Fade;
            Easing = EasingMethod.swing;
            Direction = SlideDirection.Horizontal;
            AnimationLoop = true;
            Slideshow = true;
            SlideshowSpeed = 7000;
            AnimationSpeed = 600;
            PauseOnAction = true;
            UseCSS = true;
            Touch = true;
            ControlNav = ControlNavOption.True;
            DirectionNav = true;
            PrevText = "Previous";
            NextText = "Next";
            Keyboard = true;
            PauseText = "Pause";
            PlayText = "Play";
        }

        #region General

        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.MediaFolder)]
        public string MediaFolder { get; set; }

        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.ControlId)]
        public string ControlId { get; set; }

        /// <summary>
        /// Prefix string attached to the classes of all elements generated by the plugin.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.Namespace)]
        public string Namespace { get; set; }

        /// <summary>
        /// Selector Must match a simple pattern. '{container} > {slide}'
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.Selector)]
        public string Selector { get; set; }

        /// <summary>
        /// The starting slide for the slider, in array notation.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.StartAt)]
        public short StartAt { get; set; }

        /// <summary>
        /// Setup a slideshow for the slider to animate automatically.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.Slideshow)]
        public bool Slideshow { get; set; }

        /// <summary>
        /// Set the speed of the slideshow cycling, in milliseconds.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.SlideshowSpeed)]
        public int SlideshowSpeed { get; set; }

        /// <summary>
        /// Set an initialization delay, in milliseconds.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.InitDelay)]
        public int InitDelay { get; set; }

        /// <summary>
        /// Randomize slide order, on load.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.Randomize)]
        public bool Randomize { get; set; }

        /// <summary>
        /// Pause the slideshow when interacting with control elements.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.PauseOnAction)]
        public bool PauseOnAction { get; set; }

        /// <summary>
        /// Pause the slideshow when hovering over slider, then resume when no longer hovering.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.PauseOnHover)]
        public bool PauseOnHover { get; set; }

        /// <summary>
        /// Allow touch swipe navigation of the slider on enabled devices.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.Touch)]
        public bool Touch { get; set; }

        /// <summary>
        /// Will prevent use of CSS3 3D Transforms, avoiding graphical glitches.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.Video)]
        public bool Video { get; set; }

        /// <summary>
        /// Mirror the actions performed on this slider with another slider.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.Sync)]
        public string Sync { get; set; }

        /// <summary>
        /// Turn the slider into a thumbnail navigation for another slider.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.AsNavFor)]
        public string AsNavFor { get; set; }

        /// <summary>
        /// Box-model width of individual carousel items, including horizontal borders and padding.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.ItemWidth)]
        public int ItemWidth { get; set; }

        /// <summary>
        /// Margin between carousel items.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.ItemMargin)]
        public int ItemMargin { get; set; }

        /// <summary>
        /// Minimum number of carousel items that should be visible.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.MinItems)]
        public int MinItems { get; set; }

        /// <summary>
        /// Maximum number of carousel items that should be visible.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.MaxItems)]
        public int MaxItems { get; set; }

        #endregion General

        #region Animation

        /// <summary>
        /// Controls the animation type, "fade" or "slide".
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.Animation)]
        public AnimationType Animation { get; set; }

        /// <summary>
        /// Set the speed of animations, in milliseconds.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.AnimationSpeed)]
        public int AnimationSpeed { get; set; }

        /// <summary>
        /// Controls the animation direction, "horizontal" or "vertical"
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.Direction)]
        public SlideDirection Direction { get; set; }

        /// <summary>
        /// Reverse the animation direction.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.Reverse)]
        public bool Reverse { get; set; }

        /// <summary>
        /// Number of carousel items that should move on animation.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.Move)]
        public byte Move { get; set; }

        /// <summary>
        /// Gives the slider a seamless infinite loop
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.AnimationLoop)]
        public bool AnimationLoop { get; set; }

        /// <summary>
        /// Animate the height of the slider smoothly for slides of varying height.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.SmoothHeight)]
        public bool SmoothHeight { get; set; }

        /// <summary>
        /// Determines the easing method used in jQuery transitions.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.Easing)]
        public EasingMethod Easing { get; set; }

        /// <summary>
        /// Slider will use CSS3 transitions, if available.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.UseCSS)]
        public bool UseCSS { get; set; }

        #endregion Animation

        #region Navigation

        /// <summary>
        /// Create navigation for paging control of each slide.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.ControlNav)]
        public ControlNavOption ControlNav { get; set; }

        /// <summary>
        /// Create previous/next arrow navigation.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.DirectionNav)]
        public bool DirectionNav { get; set; }

        /// <summary>
        /// Set the text for the "previous" directionNav item.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.PrevText)]
        public string PrevText { get; set; }

        /// <summary>
        /// Set the text for the "next" directionNav item.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.NextText)]
        public string NextText { get; set; }

        /// <summary>
        /// Create pause/play element to control slider slideshow.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.PausePlay)]
        public bool PausePlay { get; set; }

        /// <summary>
        /// Set the text for the "pause" pausePlay item.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.PauseText)]
        public string PauseText { get; set; }

        /// <summary>
        /// Set the text for the "play" pausePlay item.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.PlayText)]
        public string PlayText { get; set; }

        /// <summary>
        /// Container the navigation elements should be appended to.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.ControlsContainer)]
        public string ControlsContainer { get; set; }

        /// <summary>
        /// Define element to be used in lieu of dynamic controlNav.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.ManualControls)]
        public string ManualControls { get; set; }

        /// <summary>
        /// Allow slider navigating via keyboard left/right keys.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.Keyboard)]
        public bool Keyboard { get; set; }

        /// <summary>
        /// Allow keyboard navigation to affect multiple sliders.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.MultipleKeyboard)]
        public bool MultipleKeyboard { get; set; }

        /// <summary>
        /// (Dependency) Allows slider navigating via mousewheel.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.Mousewheel)]
        public bool Mousewheel { get; set; }

        #endregion Navigation

        #region Events

        /// <summary>
        /// Fires when the slider loads the first slide.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.OnStart)]
        public string OnStart { get; set; }

        /// <summary>
        /// Fires when the slider reaches the last slide (asynchronous).
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.OnEnd)]
        public string OnEnd { get; set; }

        /// <summary>
        /// Fires asynchronously with each slider animation.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.OnBefore)]
        public string OnBefore { get; set; }

        /// <summary>
        /// Fires after each slider animation completes.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.OnAfter)]
        public string OnAfter { get; set; }

        /// <summary>
        /// Fires after a slide is added.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.OnAdded)]
        public string OnAdded { get; set; }

        /// <summary>
        /// Fires after a slide is removed.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FlexSliderBlock.OnRemoved)]
        public string OnRemoved { get; set; }

        #endregion Events

        public IHtmlContent ToHtmlString()
        {
            var sb = new StringBuilder(512);

            sb.AppendFormat(@"$('#{0}').flexslider({{", ControlId);

            if (!string.IsNullOrWhiteSpace(Namespace))
            {
                sb.AppendFormat("namespace: '{0}',", Namespace);
            }
            if (!string.IsNullOrWhiteSpace(Selector))
            {
                sb.AppendFormat("selector: '{0}',", Selector);
            }
            if (StartAt > 0)
            {
                sb.AppendFormat("startAt: {0},", StartAt);
            }
            if (!Slideshow)
            {
                sb.Append("slideshow: false,");
            }
            if (SlideshowSpeed != 7000)
            {
                sb.AppendFormat("slideshowSpeed: {0},", SlideshowSpeed);
            }
            if (InitDelay > 0)
            {
                sb.AppendFormat("initDelay: {0},", InitDelay);
            }
            if (Randomize)
            {
                sb.Append("randomize: true,");
            }
            if (!PauseOnAction)
            {
                sb.Append("pauseOnAction: false,");
            }
            if (PauseOnHover)
            {
                sb.Append("pauseOnHover: true,");
            }
            if (!Touch)
            {
                sb.Append("touch: false,");
            }
            if (Video)
            {
                sb.Append("video: true,");
            }
            if (!string.IsNullOrWhiteSpace(Sync))
            {
                sb.AppendFormat("sync: '{0}',", Sync);
            }
            if (!string.IsNullOrWhiteSpace(AsNavFor))
            {
                sb.AppendFormat("asNavFor: '{0}',", AsNavFor);
            }
            if (ItemWidth > 0)
            {
                sb.AppendFormat("itemWidth: {0},", ItemWidth);
            }
            if (ItemMargin > 0)
            {
                sb.AppendFormat("itemMargin: {0},", ItemMargin);
            }
            if (MinItems > 0)
            {
                sb.AppendFormat("minItems: {0},", MinItems);
            }
            if (MaxItems > 0)
            {
                sb.AppendFormat("maxItems: {0},", MaxItems);
            }
            if (Animation != AnimationType.Fade)
            {
                sb.AppendFormat("animation: '{0}',", Animation.ToString().ToLowerInvariant());
            }
            if (AnimationSpeed != 600)
            {
                sb.AppendFormat("animationSpeed: {0},", AnimationSpeed);
            }
            if (Direction != SlideDirection.Horizontal)
            {
                sb.Append("direction: 'vertical',");
            }
            if (Reverse)
            {
                sb.Append("reverse: true,");
            }
            if (Move > 0)
            {
                sb.AppendFormat("move: {0},", Move);
            }
            if (!AnimationLoop)
            {
                sb.Append("animationLoop: false,");
            }
            if (SmoothHeight)
            {
                sb.Append("smoothHeight: true,");
            }
            if (Easing != EasingMethod.swing)
            {
                sb.AppendFormat("easing: '{0}',", Easing.ToString());
            }
            if (!UseCSS)
            {
                sb.Append("useCSS: false,");
            }

            switch (ControlNav)
            {
                case ControlNavOption.Thumbnails: sb.AppendFormat("controlNav: 'thumbnails',"); break;
                case ControlNavOption.False: sb.AppendFormat("controlNav: false,"); break;
                default: break;
            }

            if (!DirectionNav)
            {
                sb.Append("directionNav: false,");
            }
            if (!string.IsNullOrWhiteSpace(PrevText) && PrevText != "Previous")
            {
                sb.AppendFormat("prevText: '{0}',", PrevText);
            }
            if (!string.IsNullOrWhiteSpace(NextText) && PrevText != "Next")
            {
                sb.AppendFormat("nextText: '{0}',", NextText);
            }
            if (PausePlay)
            {
                sb.Append("pausePlay: true,");
            }
            if (!string.IsNullOrWhiteSpace(PauseText) && PrevText != "Pause")
            {
                sb.AppendFormat("pauseText: '{0}',", PauseText);
            }
            if (!string.IsNullOrWhiteSpace(PlayText) && PrevText != "Play")
            {
                sb.AppendFormat("playText: '{0}',", PlayText);
            }
            if (!string.IsNullOrWhiteSpace(ControlsContainer))
            {
                sb.AppendFormat("controlsContainer: '{0}',", ControlsContainer);
            }
            if (!string.IsNullOrWhiteSpace(ManualControls))
            {
                sb.AppendFormat("manualControls: '{0}',", ManualControls);
            }
            if (!Keyboard)
            {
                sb.Append("keyboard: false,");
            }
            if (MultipleKeyboard)
            {
                sb.Append("multipleKeyboard: true,");
            }
            if (Mousewheel)
            {
                sb.Append("mousewheel: true,");
            }
            if (!string.IsNullOrWhiteSpace(OnStart))
            {
                sb.AppendFormat(@"start: function(slider){{{0}}},", OnStart);
            }
            if (!string.IsNullOrWhiteSpace(OnEnd))
            {
                sb.AppendFormat(@"end: function(slider){{{0}}},", OnEnd);
            }
            if (!string.IsNullOrWhiteSpace(OnBefore))
            {
                sb.AppendFormat(@"before: function(slider){{{0}}},", OnBefore);
            }
            if (!string.IsNullOrWhiteSpace(OnAfter))
            {
                sb.AppendFormat(@"after: function(slider){{{0}}},", OnAfter);
            }
            if (!string.IsNullOrWhiteSpace(OnAdded))
            {
                sb.AppendFormat(@"added: function(slider){{{0}}},", OnAdded);
            }
            if (!string.IsNullOrWhiteSpace(OnRemoved))
            {
                sb.AppendFormat(@"removed: function(slider){{{0}}},", OnRemoved);
            }

            sb.Remove(sb.Length - 1, 1); // Remove last comma

            sb.Append("});");

            return new HtmlString(sb.ToString());
        }

        #region ContentBlockBase Overrides

        public override string Name
        {
            get { return "Flex Slider"; }
        }

        public override string DisplayTemplatePath
        {
            get { return "/Plugins/Widgets.FlexSlider/Views/Shared/DisplayTemplates/FlexSliderBlock.cshtml"; }
        }

        public override string EditorTemplatePath
        {
            get { return "/Plugins/Widgets.FlexSlider/Views/Shared/EditorTemplates/FlexSliderBlock.cshtml"; }
        }

        #endregion ContentBlockBase Overrides
    }
}