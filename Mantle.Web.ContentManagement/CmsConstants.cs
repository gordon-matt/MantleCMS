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
        internal const string BlogPosts = "Mantle_BlogPosts";
        internal const string BlogPostTags = "Mantle_BlogPostTags";
        internal const string BlogCategories = "Mantle_BlogCategories";
        internal const string BlogTags = "Mantle_BlogTags";
        internal const string ContentBlocks = "Mantle_ContentBlocks";
        internal const string EntityTypeContentBlocks = "Mantle_EntityTypeContentBlocks";
        internal const string HistoricPages = "Mantle_HistoricPages";
        internal const string MenuItems = "Mantle_MenuItems";
        internal const string Menus = "Mantle_Menus";
        internal const string MessageTemplates = "Mantle_MessageTemplates";
        internal const string Pages = "Mantle_Pages";
        internal const string PageVersions = "Mantle_PageVersions";
        internal const string PageTypes = "Mantle_PageTypes";
        internal const string SitemapConfig = "Mantle_SitemapConfig";
        internal const string QueuedEmails = "Mantle_QueuedEmails";
        internal const string QueuedSMS = "Mantle_QueuedSMS";
        internal const string Zones = "Mantle_Zones";
    }
}