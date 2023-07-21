using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mantle.Web.Mvc.MantleUI
{
    public interface IThumbnailProvider
    {
        void BeginThumbnail(Thumbnail thumbnail, TextWriter writer);

        void BeginCaptionPanel(TextWriter writer);

        void EndCaptionPanel(TextWriter writer);

        void EndThumbnail(Thumbnail thumbnail, TextWriter writer);

        void WriteThumbnailImage(Thumbnail thumbnail, IUrlHelper url, TextWriter writer);

        IHtmlContent Thumbnail(IHtmlHelper html, string src, string alt, string href, object aHtmlAttributes = null, object imgHtmlAttributes = null);
    }
}