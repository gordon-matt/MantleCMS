using Mantle.Localization;

namespace Mantle.Plugins.Widgets.View360.Infrastructure
{
    public class LanguagePackInvariant : ILanguagePack
    {
        #region ILanguagePack Members

        public string CultureCode => null;

        public IDictionary<string, string> LocalizedStrings => new Dictionary<string, string>
        {
            { LocalizableStrings.EditorTabs.General, "General" },
            { LocalizableStrings.EditorTabs.LoaderInfoConfig, "Loader Info Config" },
            { LocalizableStrings.EditorTabs.MainConfiguration, "Main Config" },
            { LocalizableStrings.EditorTabs.Navigation, "Navigation" },

            { LocalizableStrings.Mode, "Mode" },
            { LocalizableStrings.ImagesPattern, "Images Pattern" },
            { LocalizableStrings.ImagesDirectory, "Images Directory" },
            { LocalizableStrings.FullSizeImagesDirectory, "Full Size Images Directory" },

            { LocalizableStrings.AutoRotate, "Auto Rotate" },
            { LocalizableStrings.AutoRotateDirection, "Auto Rotate: Direction" },
            { LocalizableStrings.AutoRotateSpeed, "Auto Rotate: Speed" },
            { LocalizableStrings.AutoRotateStopOnMove, "Auto Rotate: Stop on Move" },
            { LocalizableStrings.ZoomMultipliers, "Zoom Multipliers" },
            { LocalizableStrings.LoadFullSizeImagesOnZoom, "Load Full Size Images on Zoom" },
            { LocalizableStrings.LoadFullSizeImagesOnFullscreen, "Load Full Size Images on Fullscreen" },
            { LocalizableStrings.Width, "Width" },
            { LocalizableStrings.Height, "Height" },
            { LocalizableStrings.Rows, "Rows" },
            { LocalizableStrings.Columns, "Columns" },
            { LocalizableStrings.XAxisSensitivity, "X-Axis Sensitivity" },
            { LocalizableStrings.YAxisSensitivity, "Y-Axis Sensitivity" },
            { LocalizableStrings.InertiaConstant, "Inertia Constant" },

            { LocalizableStrings.ButtonWidth, "Button Width" },
            { LocalizableStrings.ButtonHeight, "Button Height" },
            { LocalizableStrings.ButtonMargin, "Button Margin" },
            { LocalizableStrings.TurnSpeed, "Turn Speed" },
            { LocalizableStrings.ShowButtons, "Show Buttons" },
            { LocalizableStrings.ShowTool, "Show Tool" },
            { LocalizableStrings.ShowPlay, "Show Play" },
            { LocalizableStrings.ShowPause, "Show Pause" },
            { LocalizableStrings.ShowZoom, "Show Zoom" },
            { LocalizableStrings.ShowTurn, "Show Turn" },
            { LocalizableStrings.ShowFullscreen, "Show Fullscreen" },

            { LocalizableStrings.DisplayLoader, "Display Loader" },
            { LocalizableStrings.LoaderHolderClassName, "Loader: Holder Class Name" },
            { LocalizableStrings.LoadingTitle, "Loading Title" },
            { LocalizableStrings.LoadingSubtitle, "Loading Subtitle" },
            { LocalizableStrings.LoadingMessage, "Loading Message" },
            { LocalizableStrings.LoaderModalBackground, "Loader: Modal Background" },
            { LocalizableStrings.LoaderModalOpacity, "Loader: Modal Opacity" },
            { LocalizableStrings.LoaderCircleWidth, "Loader: Circle Width" },
            { LocalizableStrings.LoaderCircleLineWidth, "Loader: Circle Line Width" },
            { LocalizableStrings.LoaderCircleLineColor, "Loader: Circle Line Color" },
            { LocalizableStrings.LoaderCircleBackgroundColor, "Loader: Circle Background Color" }
        };

        #endregion ILanguagePack Members
    }
}