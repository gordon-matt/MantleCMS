//No license, but credit to Kazi Manzur Rashid
//http://weblogs.asp.net/rashid/archive/2009/04/20/adrotator-for-asp-net-mvc.aspx

using System;
using System.Collections.Generic;
using System.Linq;
using Mantle.Web.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace Mantle.Web.Mvc.Controls
{
    public class Ad
    {
        public string NavigateUrl { get; set; }

        public string Target { get; set; }

        public object LinkAttributes { get; set; }

        public string ImageUrl { get; set; }

        public string AlternateText { get; set; }

        public object ImageAttributes { get; set; }

        public string Keyword { get; set; }

        public int Impressions { get; set; }

        public Ad()
        {
            Target = "_blank";
        }

        public static IHtmlContent Rotate(string keywordFilter, params Ad[] ads)
        {
            Ad ad = PickAd(keywordFilter, ads);

            string html = (ad == null) ? string.Empty : GenerateHtml(ad);

            return new HtmlString(html);
        }

        public static IHtmlContent Rotate(params Ad[] ads)
        {
            return Rotate(null, ads);
        }

        private static Ad PickAd(string keywordFilter, params Ad[] ads)
        {
            Ad targetAd = null;

            IList<Ad> matchedAds = ads
                .Where(ad => string.Compare(ad.Keyword, keywordFilter, StringComparison.OrdinalIgnoreCase) == 0)
                .OrderBy(ad => ad.Impressions)
                .ToList();

            if (matchedAds.Count > 0)
            {
                int max = matchedAds.Sum(ad => ad.Impressions);
                int random = new Random().Next(max + 1);
                int runningTotal = 0;

                foreach (Ad ad in matchedAds)
                {
                    runningTotal += ad.Impressions;

                    if (random <= runningTotal)
                    {
                        targetAd = ad;
                        break;
                    }
                }

                if (targetAd == null)
                {
                    targetAd = matchedAds.Last();
                }
            }

            return targetAd;
        }

        private static string GenerateHtml(Ad ad)
        {
            Action<TagBuilder, object> merge = (builder, values) =>
            {
                if (values != null)
                {
                    builder.MergeAttributes(new RouteValueDictionary(values));
                }
            };

            Action<TagBuilder, string, string> mergeIfNotBlank = (builder, name, value) =>
            {
                if (!string.IsNullOrEmpty(value))
                {
                    builder.MergeAttribute(name, value, true);
                }
            };

            TagBuilder imageBuilder = new TagBuilder("img");
            imageBuilder.TagRenderMode = TagRenderMode.SelfClosing;

            merge(imageBuilder, ad.ImageAttributes);
            mergeIfNotBlank(imageBuilder, "src", ad.ImageUrl);
            mergeIfNotBlank(imageBuilder, "alt", ad.AlternateText);

            if (!imageBuilder.Attributes.ContainsKey("alt"))
            {
                imageBuilder.Attributes.Add("alt", string.Empty);
            }

            TagBuilder linkBuilder = new TagBuilder("a");

            merge(linkBuilder, ad.LinkAttributes);
            mergeIfNotBlank(linkBuilder, "href", ad.NavigateUrl);
            mergeIfNotBlank(linkBuilder, "target", ad.Target);

            linkBuilder.InnerHtml.AppendHtml(imageBuilder.Build());

            return linkBuilder.Build();
        }
    }
}