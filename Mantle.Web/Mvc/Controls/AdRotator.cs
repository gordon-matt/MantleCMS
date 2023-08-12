//No license, but credit to Kazi Manzur Rashid
//http://weblogs.asp.net/rashid/archive/2009/04/20/adrotator-for-asp-net-mvc.aspx

namespace Mantle.Web.Mvc.Controls;

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
        var ad = PickAd(keywordFilter, ads);

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

        var matchedAds = ads
            .Where(ad => string.Compare(ad.Keyword, keywordFilter, StringComparison.OrdinalIgnoreCase) == 0)
            .OrderBy(ad => ad.Impressions)
            .ToList();

        if (matchedAds.Count > 0)
        {
            int max = matchedAds.Sum(ad => ad.Impressions);
            int random = new Random().Next(max + 1);
            int runningTotal = 0;

            foreach (var ad in matchedAds)
            {
                runningTotal += ad.Impressions;

                if (random <= runningTotal)
                {
                    targetAd = ad;
                    break;
                }
            }

            targetAd ??= matchedAds.Last();
        }

        return targetAd;
    }

    private static string GenerateHtml(Ad ad)
    {
        static void merge(TagBuilder builder, object values)
        {
            if (values != null)
            {
                builder.MergeAttributes(new RouteValueDictionary(values));
            }
        }

        static void mergeIfNotBlank(TagBuilder builder, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                builder.MergeAttribute(name, value, true);
            }
        }

        var imageBuilder = new TagBuilder("img");
        imageBuilder.TagRenderMode = TagRenderMode.SelfClosing;

        merge(imageBuilder, ad.ImageAttributes);
        mergeIfNotBlank(imageBuilder, "src", ad.ImageUrl);
        mergeIfNotBlank(imageBuilder, "alt", ad.AlternateText);

        if (!imageBuilder.Attributes.ContainsKey("alt"))
        {
            imageBuilder.Attributes.Add("alt", string.Empty);
        }

        var linkBuilder = new TagBuilder("a");

        merge(linkBuilder, ad.LinkAttributes);
        mergeIfNotBlank(linkBuilder, "href", ad.NavigateUrl);
        mergeIfNotBlank(linkBuilder, "target", ad.Target);

        linkBuilder.InnerHtml.AppendHtml(imageBuilder.Build());

        return linkBuilder.Build();
    }
}