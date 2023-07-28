using Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Domain;
using System.Xml.Serialization;

namespace Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Models;

[XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
public class SitemapXmlFile
{
    public SitemapXmlFile()
    {
        Urls = new HashSet<UrlElement>();
    }

    [XmlElement("url")]
    public HashSet<UrlElement> Urls { get; set; }
}

public class UrlElement
{
    public UrlElement()
    {
        Links = new List<LinkElement>();
    }

    [XmlElement("loc")]
    public string Location { get; set; }

    /// <summary>
    /// Format: dd-MM-yyyy
    /// </summary>
    [XmlElement("lastmod")]
    public string LastModified { get; set; }

    [XmlElement("changefreq")]
    public ChangeFrequency ChangeFrequency { get; set; }

    [XmlElement("priority")]
    public float Priority { get; set; }

    [XmlElement("link", Namespace = "http://www.w3.org/1999/xhtml")]
    public List<LinkElement> Links { get; set; }
}

public class LinkElement
{
    [XmlAttribute("rel")]
    public string Rel { get; set; }

    [XmlAttribute("hreflang")]
    public string HrefLang { get; set; }

    [XmlAttribute("href")]
    public string Href { get; set; }
}