using System.IO;
using Extenso.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mantle.Web.Mvc.MantleUI
{
    public class Bootstrap3ThumbnailProvider : IThumbnailProvider
    {
        private readonly IUrlHelper urlHelper;

        public Bootstrap3ThumbnailProvider()
        {
            urlHelper = MvcHelpers.UrlHelper;
        }

        #region IThumbnailProvider Members

        public void BeginThumbnail(Thumbnail thumbnail, TextWriter writer)
        {
            thumbnail.EnsureClass("thumbnail");

            var builder = new TagBuilder("div");
            builder.TagRenderMode = TagRenderMode.StartTag;
            builder.MergeAttributes<string, object>(thumbnail.HtmlAttributes);
            string tag = builder.Build();

            writer.Write(tag);
        }

        public void BeginCaptionPanel(TextWriter writer)
        {
            writer.Write(@"<div class=""caption"">");
        }

        public void EndCaptionPanel(TextWriter writer)
        {
            writer.Write("</div>");
        }

        public void EndThumbnail(Thumbnail thumbnail, TextWriter writer)
        {
            writer.Write("</div>");
        }

        public void WriteThumbnailImage(Thumbnail thumbnail, IUrlHelper url, TextWriter writer)
        {
            writer.Write(string.Format(
                @"<img src=""{0}"" alt=""{1}"" />",
                url.Content(thumbnail.ImageSource),
                thumbnail.ImageAltText));
        }

        public IHtmlContent Thumbnail(IHtmlHelper html, string src, string alt, string href, object aHtmlAttributes = null, object imgHtmlAttributes = null)
        {
            var builder = new FluentTagBuilder("a")
                .MergeAttribute("href", href)
                .MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(aHtmlAttributes))
                .MergeAttribute("class", "thumbnail")
                .StartTag("img", TagRenderMode.SelfClosing)
                    .MergeAttribute("src", urlHelper.Content(src))
                    .MergeAttribute("alt", alt)
                    .MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(imgHtmlAttributes))
                .EndTag();

            return new HtmlString(builder.ToString());
        }

        #endregion IThumbnailProvider Members
    }
}