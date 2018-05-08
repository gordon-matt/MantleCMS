using System.Text;
using Mantle.Localization.ComponentModel;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.ContentBlocks
{
    public class TagCloudBlock : ContentBlockBase
    {
        public TagCloudBlock()
        {
            //Width = 100;
            //WidthUnit = CssUnit.Percentage;
            //Height = 250;
            //HeightUnit = CssUnit.Pixels;
            //CenterX = 0.5f;
            //CenterY = 0.5f;
            AutoResize = false;
            Steps = 10;
            ClassPattern = "w{n}";
            Shape = CloudShape.Elliptic;
            RemoveOverflowing = true;
            EncodeURI = true;
        }

        ///// <summary>
        ///// Fixed width of the container, will default to container current width.
        ///// </summary>
        //[LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.Width)]
        //public short Width { get; set; }

        //[LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.WidthUnit)]
        //public CssUnit WidthUnit { get; set; }

        ///// <summary>
        ///// Fixed height of the container, will default to container current height.
        ///// </summary>
        //[LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.Height)]
        //public short Height { get; set; }

        //[LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.HeightUnit)]
        //public CssUnit HeightUnit { get; set; }

        ///// <summary>
        ///// Position of the center of the cloud relatively to the container.
        ///// </summary>
        //[LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.CenterX)]
        //public float CenterX { get; set; }

        ///// <summary>
        ///// Position of the center of the cloud relatively to the container.
        ///// </summary>
        //[LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.CenterY)]
        //public float CenterY { get; set; }

        /// <summary>
        /// If the container has dynamic dimensions, set this option to true to update the cloud when the window is resized.
        /// </summary>
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.AutoResize)]
        public bool AutoResize { get; set; }

        /// <summary>
        /// Number of "steps" to map the words on, depending on their weight (see Colors & sizes section).
        /// </summary>
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.Steps)]
        public byte Steps { get; set; }

        /// <summary>
        /// Pattern used to generate the CSS class added to each word. {n} is replaced by the weight of the word (from 1 to steps).
        /// </summary>
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.ClassPattern)]
        public string ClassPattern { get; set; }

        /// <summary>
        /// Function called after the whole cloud is fully rendered. this is the container jQuery object.
        /// </summary>
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.AfterCloudRender)]
        public string AfterCloudRender { get; set; }

        /// <summary>
        /// <para>Number of milliseconds to wait between each word draw. Defaults to 10 if there are 50 words or more to avoid</para>
        /// <para>browser freezing during rendering. Can also be used for neat display animation!</para>
        /// </summary>
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.Delay)]
        public byte? Delay { get; set; }

        /// <summary>
        /// Cloud shape
        /// </summary>
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.Shape)]
        public CloudShape Shape { get; set; }

        /// <summary>
        /// Don't render words which would overflow the container.
        /// </summary>
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.RemoveOverflowing)]
        public bool RemoveOverflowing { get; set; }

        /// <summary>
        /// Encode special characters in words link.
        /// </summary>
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.EncodeURI)]
        public bool EncodeURI { get; set; }

        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.Colors)]
        public string Colors { get; set; }

        /// <summary>
        /// Expressed in fraction of container width
        /// </summary>
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.FontSizeFrom)]
        public float? FontSizeFrom { get; set; }

        /// <summary>
        /// Expressed in fraction of container width
        /// </summary>
        [LocalizedDisplayName(MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.FontSizeTo)]
        public float? FontSizeTo { get; set; }

        #region ContentBlockBase Overrides

        public override string Name => "Blog: Tag Cloud";

        public override string DisplayTemplatePath => "Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.Shared.DisplayTemplates.TagCloudBlock.cshtml";

        public override string EditorTemplatePath => "Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.Shared.EditorTemplates.TagCloudBlock.cshtml";

        #endregion ContentBlockBase Overrides

        public string GetOptions()
        {
            var sb = new StringBuilder(512);

            sb.Append("{ ");
            //sb.AppendFormat("width:'{0}{1}',", Width, WidthUnit == CssUnit.Percentage ? "%" : "px");
            //sb.AppendFormat("height:'{0}{1}',", Height, HeightUnit == CssUnit.Percentage ? "%" : "px");

            //if (CenterX != 0.5f || CenterY != 0.5f)
            //{
            //    sb.AppendFormat("center:{{x:{0},y:{1}}},", CenterX, CenterY);
            //}
            if (AutoResize)
            {
                sb.Append("autoResize:true,");
            }
            if (Steps != 10)
            {
                sb.AppendFormat("steps:{0},", Steps);
            }
            if (ClassPattern != "w{n}")
            {
                sb.AppendFormat("classPattern:'{0}',", ClassPattern);
            }
            if (!string.IsNullOrWhiteSpace(AfterCloudRender))
            {
                sb.AppendFormat("afterCloudRender:function(){{ {0} }},", AfterCloudRender);
            }
            if (Delay.HasValue)
            {
                sb.AppendFormat("delay:{0},", Delay);
            }
            if (Shape != CloudShape.Elliptic)
            {
                sb.AppendFormat("shape:'{0}',", Shape.ToString().ToLowerInvariant());
            }
            if (!RemoveOverflowing)
            {
                sb.Append("removeOverflowing:false,");
            }
            if (!EncodeURI)
            {
                sb.Append("encodeURI:false,");
            }
            if (!string.IsNullOrWhiteSpace(Colors))
            {
                sb.AppendFormat("colors:[{0}],", string.Join("','", Colors.Split(',')).AddSingleQuotes());
            }
            if (FontSizeFrom.HasValue && FontSizeTo.HasValue)
            {
                sb.AppendFormat("fontSize:{{from: {0},to: {1} }},", FontSizeFrom.Value, FontSizeTo.Value);
            }

            sb.Remove(sb.Length - 1, 1); // Remove last comma

            sb.Append("}");

            return sb.ToString();
        }
    }

    public enum CloudShape : byte
    {
        Elliptic = 0,
        Rectangular = 1
    }
}