using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace Mantle.Web.Html
{
    public static class HtmlContentExtensions
    {
        public static string GetString(this IHtmlContent htmlContent)
        {
            using (var stringWriter = new StringWriter())
            {
                htmlContent.WriteTo(stringWriter, HtmlEncoder.Default);
                return stringWriter.ToString();
            }
        }
    }
}