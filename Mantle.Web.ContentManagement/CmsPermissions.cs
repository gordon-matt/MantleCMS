namespace Mantle.Web.ContentManagement;

public class CmsPermissions : IPermissionProvider
{
    #region Blog

    public static readonly Permission BlogRead = new() { Name = "Blog_Read", Category = "Content Management", Description = "Blog: Read" };
    public static readonly Permission BlogWrite = new() { Name = "Blog_Write", Category = "Content Management", Description = "Blog: Write" };

    #endregion Blog

    #region Content Blocks

    public static readonly Permission ContentBlocksRead = new() { Name = "ContentBlocks_Read", Category = "Content Management", Description = "Content Blocks: Read" };
    public static readonly Permission ContentBlocksWrite = new() { Name = "ContentBlocks_Write", Category = "Content Management", Description = "Content Blocks: Write" };
    public static readonly Permission ContentZonesRead = new() { Name = "ContentZones_Read", Category = "Content Management", Description = "Content Zones: Read" };
    public static readonly Permission ContentZonesWrite = new() { Name = "ContentZones_Write", Category = "Content Management", Description = "Content Zones: Write" };

    #endregion Content Blocks

    #region Localization

    public static readonly Permission LanguagesRead = new() { Name = "Languages_Read", Category = "Content Management", Description = "Languages: Read" };
    public static readonly Permission LanguagesWrite = new() { Name = "Languages_Write", Category = "Content Management", Description = "Languages: Write" };
    public static readonly Permission LocalizableStringsRead = new() { Name = "LocalizableStrings_Read", Category = "Content Management", Description = "Localizable Strings: Read" };
    public static readonly Permission LocalizableStringsWrite = new() { Name = "LocalizableStrings_Write", Category = "Content Management", Description = "Localizable Strings: Write" };

    #endregion Localization

    #region Media

    public static readonly Permission MediaRead = new() { Name = "Media_Read", Category = "Content Management", Description = "Media: Read" };
    public static readonly Permission MediaWrite = new() { Name = "Media_Write", Category = "Content Management", Description = "Media: Write" };

    #endregion Media

    #region Menus

    public static readonly Permission MenusRead = new() { Name = "Menus_Read", Category = "Content Management", Description = "Menus: Read" };
    public static readonly Permission MenusWrite = new() { Name = "Menus_Write", Category = "Content Management", Description = "Menus: Write" };

    #endregion Menus

    #region Newsletter

    public static readonly Permission NewsletterRead = new() { Name = "Newsletter_Read", Category = "Content Management", Description = "Newsletter: Read" };
    public static readonly Permission NewsletterWrite = new() { Name = "Newsletter_Write", Category = "Content Management", Description = "Newsletter: Write" };

    #endregion Newsletter

    #region Pages

    public static readonly Permission PageHistoryRead = new() { Name = "PageHistory_Read", Category = "Content Management", Description = "Page History: Read" };
    public static readonly Permission PageHistoryRestore = new() { Name = "PageHistory_Restore", Category = "Content Management", Description = "Page History: Restore" };
    public static readonly Permission PageHistoryWrite = new() { Name = "PageHistory_Write", Category = "Content Management", Description = "Page History: Write" };
    public static readonly Permission PagesRead = new() { Name = "Pages_Read", Category = "Content Management", Description = "Pages: Read" };
    public static readonly Permission PagesWrite = new() { Name = "Pages_Write", Category = "Content Management", Description = "Pages: Write" };
    public static readonly Permission PageTypesRead = new() { Name = "PageTypes_Read", Category = "Content Management", Description = "Page Types: Read" };
    public static readonly Permission PageTypesWrite = new() { Name = "PageTypes_Write", Category = "Content Management", Description = "Page Types: Read" };

    #endregion Pages

    #region Sitemap

    public static readonly Permission SitemapRead = new() { Name = "Sitemap_Read", Category = "Content Management", Description = "Sitemap - Read" };
    public static readonly Permission SitemapWrite = new() { Name = "Sitemap_Write", Category = "Content Management", Description = "Sitemap - Write" };

    #endregion Sitemap

    public IEnumerable<Permission> GetPermissions()
    {
        yield return BlogRead;
        yield return BlogWrite;

        yield return ContentBlocksRead;
        yield return ContentBlocksWrite;
        yield return ContentZonesRead;
        yield return ContentZonesWrite;

        yield return LanguagesRead;
        yield return LanguagesWrite;
        yield return LocalizableStringsRead;
        yield return LocalizableStringsWrite;

        yield return MediaRead;
        yield return MediaWrite;

        yield return MenusRead;
        yield return MenusWrite;

        yield return NewsletterRead;
        yield return NewsletterWrite;

        yield return PageHistoryRead;
        yield return PageHistoryRestore;
        yield return PageHistoryWrite;
        yield return PagesRead;
        yield return PagesWrite;
        yield return PageTypesRead;
        yield return PageTypesWrite;

        yield return SitemapRead;
        yield return SitemapWrite;
    }
}