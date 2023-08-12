using System.Text.RegularExpressions;

namespace Mantle.Web.ContentManagement;

public static class CmsConstants
{
    public static class RegexPatterns
    {
        public static readonly Regex Email = new("^[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,4}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    public static class Areas
    {
        public const string Blocks = "Admin/Blocks";
        public const string Blog = "Admin/Blog";
        public const string Localization = "Admin/Localization";
        public const string Media = "Admin/Media";
        public const string Messaging = "Admin/Messaging";
        public const string Menus = "Admin/Menus";
        public const string Newsletters = "Admin/Newsletters";
        public const string Pages = "Admin/Pages";
        public const string Sitemap = "Admin/Sitemap";
    }

    public static class CacheKeys
    {
        public const string ContentBlockScriptExpression = "Mantle_CMS_Blocks_ScriptExpression_{0}";
        public const string MediaMimeType = "Mantle_CMS_Media_MimType_{0}";
        public const string MediaImageEntityTypesAll = "Mantle_CMS_Media_ImageEntityTypes_All";
    }

    internal static class Tables
    {
        internal const string BlogPosts = "BlogPosts";
        internal const string BlogPostTags = "BlogPostTags";
        internal const string BlogCategories = "BlogCategories";
        internal const string BlogTags = "BlogTags";
        internal const string ContentBlocks = "ContentBlocks";
        internal const string EntityTypeContentBlocks = "EntityTypeContentBlocks";
        internal const string HistoricPages = "HistoricPages";
        internal const string MenuItems = "MenuItems";
        internal const string Menus = "Menus";
        internal const string MessageTemplates = "MessageTemplates";
        internal const string Pages = "Pages";
        internal const string PageVersions = "PageVersions";
        internal const string PageTypes = "PageTypes";
        internal const string SitemapConfig = "SitemapConfig";
        internal const string QueuedEmails = "QueuedEmails";
        internal const string QueuedSMS = "QueuedSMS";
        internal const string Zones = "Zones";
    }
}