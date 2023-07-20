using Mantle.Security.Membership.Permissions;

namespace Mantle.Web.ContentManagement
{
    public class CmsPermissions : IPermissionProvider
    {
        #region Blog

        public static readonly Permission BlogRead = new Permission { Name = "Blog_Read", Category = "Content Management", Description = "Blog: Read" };
        public static readonly Permission BlogWrite = new Permission { Name = "Blog_Write", Category = "Content Management", Description = "Blog: Write" };

        #endregion Blog

        #region Content Blocks

        public static readonly Permission ContentBlocksRead = new Permission { Name = "ContentBlocks_Read", Category = "Content Management", Description = "Content Blocks: Read" };
        public static readonly Permission ContentBlocksWrite = new Permission { Name = "ContentBlocks_Write", Category = "Content Management", Description = "Content Blocks: Write" };
        public static readonly Permission ContentZonesRead = new Permission { Name = "ContentZones_Read", Category = "Content Management", Description = "Content Zones: Read" };
        public static readonly Permission ContentZonesWrite = new Permission { Name = "ContentZones_Write", Category = "Content Management", Description = "Content Zones: Write" };

        #endregion Content Blocks

        #region Localization

        public static readonly Permission LanguagesRead = new Permission { Name = "Languages_Read", Category = "Content Management", Description = "Languages: Read" };
        public static readonly Permission LanguagesWrite = new Permission { Name = "Languages_Write", Category = "Content Management", Description = "Languages: Write" };
        public static readonly Permission LocalizableStringsRead = new Permission { Name = "LocalizableStrings_Read", Category = "Content Management", Description = "Localizable Strings: Read" };
        public static readonly Permission LocalizableStringsWrite = new Permission { Name = "LocalizableStrings_Write", Category = "Content Management", Description = "Localizable Strings: Write" };

        #endregion Localization

        #region Media

        public static readonly Permission MediaRead = new Permission { Name = "Media_Read", Category = "Content Management", Description = "Media: Read" };
        public static readonly Permission MediaWrite = new Permission { Name = "Media_Write", Category = "Content Management", Description = "Media: Write" };

        #endregion Media

        #region Menus

        public static readonly Permission MenusRead = new Permission { Name = "Menus_Read", Category = "Content Management", Description = "Menus: Read" };
        public static readonly Permission MenusWrite = new Permission { Name = "Menus_Write", Category = "Content Management", Description = "Menus: Write" };

        #endregion Menus

        #region Newsletter

        public static readonly Permission NewsletterRead = new Permission { Name = "Newsletter_Read", Category = "Content Management", Description = "Newsletter: Read" };
        public static readonly Permission NewsletterWrite = new Permission { Name = "Newsletter_Write", Category = "Content Management", Description = "Newsletter: Write" };

        #endregion Newsletter

        #region Pages

        public static readonly Permission PageHistoryRead = new Permission { Name = "PageHistory_Read", Category = "Content Management", Description = "Page History: Read" };
        public static readonly Permission PageHistoryRestore = new Permission { Name = "PageHistory_Restore", Category = "Content Management", Description = "Page History: Restore" };
        public static readonly Permission PageHistoryWrite = new Permission { Name = "PageHistory_Write", Category = "Content Management", Description = "Page History: Write" };
        public static readonly Permission PagesRead = new Permission { Name = "Pages_Read", Category = "Content Management", Description = "Pages: Read" };
        public static readonly Permission PagesWrite = new Permission { Name = "Pages_Write", Category = "Content Management", Description = "Pages: Write" };
        public static readonly Permission PageTypesRead = new Permission { Name = "PageTypes_Read", Category = "Content Management", Description = "Page Types: Read" };
        public static readonly Permission PageTypesWrite = new Permission { Name = "PageTypes_Write", Category = "Content Management", Description = "Page Types: Read" };

        #endregion Pages

        #region Sitemap

        public static readonly Permission SitemapRead = new Permission { Name = "Sitemap_Read", Category = "Content Management", Description = "Sitemap - Read" };
        public static readonly Permission SitemapWrite = new Permission { Name = "Sitemap_Write", Category = "Content Management", Description = "Sitemap - Write" };

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
}