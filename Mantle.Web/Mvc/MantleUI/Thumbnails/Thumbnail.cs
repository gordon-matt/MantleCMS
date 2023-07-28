namespace Mantle.Web.Mvc.MantleUI;

public class Thumbnail : HtmlElement
{
    public string ImageSource { get; set; }

    public string ImageAltText { get; set; }

    public Thumbnail(string src, string alt, object divHtmlAttributes = null, object imgHtmlAttributes = null)
        : base(divHtmlAttributes)
    {
        this.ImageSource = src;
        this.ImageAltText = alt;
    }

    protected internal override void StartTag(TextWriter textWriter)
    {
        Provider.ThumbnailProvider.BeginThumbnail(this, textWriter);
    }

    protected internal override void EndTag(TextWriter textWriter)
    {
        Provider.ThumbnailProvider.EndThumbnail(this, textWriter);
    }
}