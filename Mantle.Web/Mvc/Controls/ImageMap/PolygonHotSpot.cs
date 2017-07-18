//No license, but credit to Richard Lee:
//http://www.avantprime.com/blog/9/asp-net-mvc-image-map-helper

using System.Drawing;
using System.Text;

namespace Mantle.Web.Mvc.Controls
{
    public class PolygonHotSpot : ImageMapHotSpot
    {
        public Point[] Coordinates { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < Coordinates.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(",");
                }

                sb.AppendFormat("{0},{1}", Coordinates[i].X, Coordinates[i].Y);
            }
            return string.Format(@"<area shape=""polygon"" coords=""{0}"" href=""{1}"" alt=""{2}"" />", sb, Url, Title);
        }
    }
}