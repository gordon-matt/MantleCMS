namespace Mantle.Web.ContentManagement.Areas.Admin.Media.Models;

public class MediaFile
{
    public string Name { get; set; }

    public string Path { get; set; }

    public string ThumbPath { get; set; }

    public string DirectoryPath { get; set; }

    public string Type { get; set; }

    public long Size { get; set; }

    public DateTime LastUpdated { get; set; }
}