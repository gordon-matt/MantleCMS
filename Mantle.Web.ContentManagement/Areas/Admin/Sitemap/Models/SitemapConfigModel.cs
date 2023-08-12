using Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Entities;

namespace Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Models;

public class SitemapConfigModel
{
    public int Id { get; set; }

    public string Location { get; set; }

    public ChangeFrequency ChangeFrequency { get; set; }

    /// <summary>
    /// From 0.0 to 1.0
    /// </summary>
    public float Priority { get; set; }
}