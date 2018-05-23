using Mantle.Localization.ComponentModel;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Plugins.Widgets.View360
{
    public class View360Block : ContentBlockBase
    {
        public View360Block()
        {
            Mode = Mode.Fixed;
            ImagesPattern = "image-%COL-%ROW.jpg";
            AutoRotateDirection = AutoRotateDirection.Right;
            AutoRotateSpeed = 50;
            AutoRotateStopOnMove = true;
            //ZoomMultipliers = new List<float> { 1f, 1.2f, 1.5f, 2f, 3f };
            LoadFullSizeImagesOnZoom = true;
            LoadFullSizeImagesOnFullscreen = true;
            Width = 620;
            Height = 350;
            Rows = 1;
            Columns = 36;
            XAxisSensitivity = 10;
            YAxisSensitivity = 40;
            InertiaConstant = 10;

            ButtonWidth = 40;
            ButtonHeight = 40;
            ButtonMargin = 5;
            TurnSpeed = 40;
            ShowButtons = true;
            ShowTool = true;
            ShowPlay = true;
            ShowPause = true;
            ShowZoom = true;
            ShowTurn = true;
            ShowFullscreen = true;

            DisplayLoader = true;
            LoaderModalBackground = "#FFF";
            LoaderModalOpacity = 0.5f;
            LoaderCircleWidth = 70;
            LoaderCircleLineWidth = 10;
            LoaderCircleLineColor = "#555";
            LoaderCircleBackgroundColor = "#FFF";
        }

        #region General

        /// <summary>
        /// Sets visual appearance. Options:fixed, lightbox, fullview and responsive.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.Mode)]
        public Mode Mode { get; set; }

        /// <summary>
        ///  If we have hundreds of product, we must standardize product names in order to dynamically display products without worry
        ///  about images names.
        ///  Imagine we have 36 images for full rotation: image-0-0.jpg, image-1-0.jpg, .... image-35-0.jpg
        ///  We can simply set pattern image-%COL-%ROW.jpg and component will auto load images without need to pass array of images names.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.ImagesPattern)]
        public string ImagesPattern { get; set; }

        /// <summary>
        /// Path to images directory.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.ImagesDirectory)]
        public string ImagesDirectory { get; set; }

        /// <summary>
        /// Path to full sized images. Tease images will be loaded product zoom.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.FullSizeImagesDirectory)]
        public string FullSizeImagesDirectory { get; set; }

        #endregion General

        #region Main Configuration

        /// <summary>
        /// auto rotate on init
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.AutoRotate)]
        public bool AutoRotate { get; set; }

        /// <summary>
        /// auto rotate direction
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.AutoRotateDirection)]
        public AutoRotateDirection AutoRotateDirection { get; set; }

        /// <summary>
        /// auto rotate speed
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.AutoRotateSpeed)]
        public int AutoRotateSpeed { get; set; }

        /// <summary>
        /// stop auto rotation on user interaction
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.AutoRotateStopOnMove)]
        public bool AutoRotateStopOnMove { get; set; }

        //TODO: Changing this causes the viewer to be blank when zooming in and out. For now, leave the defaults until we
        //  can figure out how to fix this. It's not an important option to have anyway.
        ///// <summary>
        ///// array of zoom multipliers
        ///// </summary>
        //[LocalizedDisplayName(LocalizableStrings.ZoomMultipliers)]
        //public List<float> ZoomMultipliers { get; set; }

        /// <summary>
        /// If set to true, full size images will be loaded on first zoom. Also this property can set to one of the vaues
        /// from zoomMultipliers array. Full size images will be loaded on certan zoom multiplier.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.LoadFullSizeImagesOnZoom)]
        public bool LoadFullSizeImagesOnZoom { get; set; }

        /// <summary>
        /// If set to true, fullscreen button starts full size images loading.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.LoadFullSizeImagesOnFullscreen)]
        public bool LoadFullSizeImagesOnFullscreen { get; set; }

        /// <summary>
        /// View width
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.Width)]
        public short Width { get; set; }

        /// <summary>
        /// View height
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.Height)]
        public short Height { get; set; }

        /// <summary>
        /// View width
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.Rows)]
        public short Rows { get; set; }

        /// <summary>
        /// View height
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.Columns)]
        public short Columns { get; set; }

        /// <summary>
        /// Column change sensitivity in pixels
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.XAxisSensitivity)]
        public short XAxisSensitivity { get; set; }

        /// <summary>
        /// Row change sensitivity in pixels.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.YAxisSensitivity)]
        public short YAxisSensitivity { get; set; }

        /// <summary>
        /// Inertia rotation constant. Set 0 to disable.
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.InertiaConstant)]
        public short InertiaConstant { get; set; }

        #endregion Main Configuration

        #region Navigation Buttons Configuration

        /// <summary>
        /// button width
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.ButtonWidth)]
        public short ButtonWidth { get; set; }

        /// <summary>
        /// button height
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.ButtonHeight)]
        public short ButtonHeight { get; set; }

        /// <summary>
        /// distance between buttons
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.ButtonMargin)]
        public short ButtonMargin { get; set; }

        /// <summary>
        /// rotation speed for turnLeft and turnRight buttons
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.TurnSpeed)]
        public short TurnSpeed { get; set; }

        /// <summary>
        /// show navigation buttons
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.ShowButtons)]
        public bool ShowButtons { get; set; }

        [LocalizedDisplayName(LocalizableStrings.ShowTool)]
        public bool ShowTool { get; set; }

        [LocalizedDisplayName(LocalizableStrings.ShowPlay)]
        public bool ShowPlay { get; set; }

        [LocalizedDisplayName(LocalizableStrings.ShowPause)]
        public bool ShowPause { get; set; }

        [LocalizedDisplayName(LocalizableStrings.ShowZoom)]
        public bool ShowZoom { get; set; }

        [LocalizedDisplayName(LocalizableStrings.ShowTurn)]
        public bool ShowTurn { get; set; }

        [LocalizedDisplayName(LocalizableStrings.ShowFullscreen)]
        public bool ShowFullscreen { get; set; }

        #endregion Navigation Buttons Configuration

        #region Loader Info Config

        /// <summary>
        /// display loader info
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.DisplayLoader)]
        public bool DisplayLoader { get; set; }

        /// <summary>
        /// classname for css override
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.LoaderHolderClassName)]
        public string LoaderHolderClassName { get; set; }

        [LocalizedDisplayName(LocalizableStrings.LoadingTitle)]
        public string LoadingTitle { get; set; }

        [LocalizedDisplayName(LocalizableStrings.LoadingSubtitle)]
        public string LoadingSubtitle { get; set; }

        [LocalizedDisplayName(LocalizableStrings.LoadingMessage)]
        public string LoadingMessage { get; set; }

        /// <summary>
        /// Color of loader background
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.LoaderModalBackground)]
        public string LoaderModalBackground { get; set; }

        /// <summary>
        /// Opacity of loader background (range between 0-1)
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.LoaderModalOpacity)]
        public float LoaderModalOpacity { get; set; }

        /// <summary>
        /// Loader circle width
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.LoaderCircleWidth)]
        public float LoaderCircleWidth { get; set; }

        /// <summary>
        /// Loader circle line width
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.LoaderCircleLineWidth)]
        public float LoaderCircleLineWidth { get; set; }

        /// <summary>
        /// Loader circle line color
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.LoaderCircleLineColor)]
        public string LoaderCircleLineColor { get; set; }

        /// <summary>
        /// Loader circle background color
        /// </summary>
        [LocalizedDisplayName(LocalizableStrings.LoaderCircleBackgroundColor)]
        public string LoaderCircleBackgroundColor { get; set; }

        #endregion Loader Info Config

        #region ContentBlockBase Overrides

        public override string Name
        {
            get { return "View360"; }
        }

        public override string DisplayTemplatePath
        {
            get { return "/Plugins/Widgets.View360/Views/Shared/DisplayTemplates/View360Block.cshtml"; }
        }

        public override string EditorTemplatePath
        {
            get { return "/Plugins/Widgets.View360/Views/Shared/EditorTemplates/View360Block.cshtml"; }
        }

        #endregion ContentBlockBase Overrides
    }

    public enum Mode : byte
    {
        /// <summary>
        /// Used for in page display. This is default value.
        /// </summary>
        Fixed = 0,

        /// <summary>
        /// Product will be shown in lightbox (in page popup).
        /// </summary>
        Lightbox = 1,

        /// <summary>
        /// Product will be scaled 100% by width and height inside browser.
        /// </summary>
        FullView = 2,

        /// <summary>
        /// Depend on folder width, view will automatically change its size.
        /// </summary>
        Responsive = 3
    }

    public enum AutoRotateDirection : sbyte
    {
        Left = -1,
        Right = 1
    }
}