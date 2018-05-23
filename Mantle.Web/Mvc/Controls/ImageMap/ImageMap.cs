//No license, but credit to Richard Lee:
//http://www.avantprime.com/blog/9/asp-net-mvc-image-map-helper

using System.Text;
using Extenso.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mantle.Web.Mvc.Controls
{
    public class ImageMap
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public ImageMapHotSpot[] HotSpots { get; set; }

        public override string ToString()
        {
            var builder = new TagBuilder("map");
            builder.MergeAttribute("id", ID);
            builder.MergeAttribute("name", Name);

            var sb = new StringBuilder();
            foreach (var hotSpot in HotSpots)
            {
                sb.Append(hotSpot);
            }

            builder.InnerHtml.AppendHtml(sb.ToString());

            return builder.Build();
        }
    }
}