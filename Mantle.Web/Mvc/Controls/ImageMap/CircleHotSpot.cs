//No license, but credit to Richard Lee:
//http://www.avantprime.com/blog/9/asp-net-mvc-image-map-helper

using System.Drawing;

namespace Mantle.Web.Mvc.Controls;

public class CircleHotSpot : ImageMapHotSpot
{
    public Point Center { get; set; }

    public int Radius { get; set; }

    public override string ToString()
    {
        return string.Format(@"<area shape=""circle"" coords=""{0},{1},{2}"" href=""{3}"" alt=""{4}"" />",
            Center.X, Center.Y, Radius, Url, Title);
    }
}