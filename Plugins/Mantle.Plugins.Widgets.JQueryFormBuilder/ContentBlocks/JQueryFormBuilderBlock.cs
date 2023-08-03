using Mantle.Localization.ComponentModel;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Plugins.Widgets.JQueryFormBuilder.ContentBlocks
{
    public class JQueryFormBuilderBlock : ContentBlockBase
    {
        [BlockProperty]
        public string Metadata { get; set; }

        [BlockProperty]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FormBuilderBlock.ThankYouMessage)]
        public string ThankYouMessage { get; set; }

        [BlockProperty]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FormBuilderBlock.RedirectUrl)]
        public string RedirectUrl { get; set; }

        [BlockProperty]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FormBuilderBlock.EmailAddress)]
        public string EmailAddress { get; set; }

        [BlockProperty]
        [LocalizedDisplayName(LocalizableStrings.ContentBlocks.FormBuilderBlock.UseAjax)]
        public bool UseAjax { get; set; }

        public static string GenerateCaptcha()
        {
            return string.Empty;

            //var random = new Random((int)DateTime.Now.Ticks);
            ////generate new question
            //int a = random.Next(10, 99);
            //int b = random.Next(0, 50);
            //var captcha = string.Format("{0} + {1} = ?", a, b);

            //using (var memoryStream = new MemoryStream())
            //using (var bitmap = new Bitmap(130, 30))
            //using (var g = Graphics.FromImage(bitmap))
            //{
            //    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            //    g.SmoothingMode = SmoothingMode.AntiAlias;
            //    g.FillRectangle(Brushes.White, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

            //    // Add noise
            //    int i;
            //    var pen = new Pen(Color.Yellow);
            //    for (i = 1; i < 5; i++)
            //    {
            //        pen.Color = random.NextColor();

            //        var r = random.Next(0, (130 / 3));
            //        var x = random.Next(0, 130);
            //        var y = random.Next(0, 30);

            //        g.DrawEllipse(pen, x - r, y - r, r, r);
            //    }

            //    // Add question
            //    g.DrawString(captcha, new Font("Tahoma", 15), Brushes.Gray, 2, 3);

            //    //render as Jpeg
            //    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            //    var base64String = Convert.ToBase64String(memoryStream.GetBuffer());
            //    var hashedPassword = Crypto.HashPassword((a + b).ToString(CultureInfo.InvariantCulture));

            //    return string.Format("<img src=\"data:image/jpeg;base64,{0}\" class=\"form-captcha\" /><input type=\"hidden\" name=\"captcha_challenge\" value=\"{1}\" />", base64String, hashedPassword);
            //}
        }

        #region ContentBlockBase Overrides

        public override string Name => "jQuery Form Builder";

        public override string DisplayTemplatePath => "/Plugins/Widgets.JQueryFormBuilder/Views/Shared/DisplayTemplates/JQueryFormBuilderBlock.cshtml";

        public override string EditorTemplatePath => "/Plugins/Widgets.JQueryFormBuilder/Views/Shared/EditorTemplates/JQueryFormBuilderBlock.cshtml";

        #endregion ContentBlockBase Overrides
    }
}