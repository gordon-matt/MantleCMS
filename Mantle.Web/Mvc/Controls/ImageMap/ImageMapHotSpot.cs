//No license, but credit to Richard Lee:
//http://www.avantprime.com/blog/9/asp-net-mvc-image-map-helper

namespace Mantle.Web.Mvc.Controls;

public abstract class ImageMapHotSpot
{
    public string Title { get; set; }

    public string Url { get; set; }
}