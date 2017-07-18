//No license, but credit to Richard Lee:
//http://www.avantprime.com/blog/9/asp-net-mvc-image-map-helper

using System.Drawing;

namespace Mantle.Web.Mvc.Controls
{
    public class RectangleHotSpot : ImageMapHotSpot
    {
        public Point TopLeft { get; set; }

        public Point BottomRight { get; set; }

        public override string ToString()
        {
            return string.Format(@"<area shape=""rect"" coords=""{0},{1},{2},{3}"" href=""{4}"" alt=""{5}"" />",
                TopLeft.X, TopLeft.Y, BottomRight.X, BottomRight.Y, Url, Title);
        }
    }
}