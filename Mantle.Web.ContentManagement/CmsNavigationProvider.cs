using Mantle.Infrastructure;
using Mantle.Web.Navigation;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.ContentManagement
{
    public class CmsNavigationProvider : INavigationProvider
    {
        public CmsNavigationProvider()
        {
            T = EngineContext.Current.Resolve<IStringLocalizer>();
        }

        public IStringLocalizer T { get; set; }

        #region INavigationProvider Members

        public string MenuName => MantleWebConstants.Areas.Admin;

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T[MantleCmsLocalizableStrings.Navigation.CMS].Value, "2", BuildCmsMenu);
        }

        #endregion INavigationProvider Members

        private void BuildCmsMenu(NavigationItemBuilder builder)
        {
            builder.Icons("kore-icon kore-icon-cms");

            // Blog
            builder.Add(T[MantleCmsLocalizableStrings.Blog.Title].Value, "5", item => item
                .Url("#blog")
                //.Action("Index", "Blog", new { area = CmsConstants.Areas.Blog })
                .Icons("kore-icon kore-icon-blog")
                .Permission(CmsPermissions.BlogRead));

            // Content Blocks
            builder.Add(T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value, "5", item => item
                .Url("#blocks/content-blocks")
                //.Action("Index", "ContentBlock", new { area = CmsConstants.Areas.Blocks, pageId = UrlParameter.Optional })
                .Icons("kore-icon kore-icon-content-blocks")
                .Permission(CmsPermissions.ContentBlocksRead));

            // Localization
            builder.Add(T[MantleCmsLocalizableStrings.Localization.Title].Value, "5", item => item
                .Url("#localization/languages")
                //.Action("Index", "Language", new { area = CmsConstants.Areas.Localization })
                .Icons("kore-icon kore-icon-localization")
                .Permission(CmsPermissions.LanguagesRead));

            // Media
            builder.Add(T[MantleCmsLocalizableStrings.Media.Title].Value, "5", item => item
                .Url("#media")
                //.Action("Index", "Media", new { area = CmsConstants.Areas.Media })
                .Icons("kore-icon kore-icon-media")
                .Permission(CmsPermissions.MediaRead));

            // Menus
            builder.Add(T[MantleCmsLocalizableStrings.Menus.Title].Value, "5", item => item
                .Url("#menus")
                //.Action("Index", "Menu", new { area = CmsConstants.Areas.Menus })
                .Icons("kore-icon kore-icon-menus")
                .Permission(CmsPermissions.MenusRead));

            // Messaging
            builder.Add(T[MantleCmsLocalizableStrings.Messaging.MessageTemplates].Value, "5", item => item
                .Url("#messaging/templates")
                //.Action("Index", "MessageTemplate", new { area = CmsConstants.Areas.Messaging })
                .Icons("kore-icon kore-icon-message-templates")
                .Permission(CmsPermissions.MessageTemplatesRead));

            // Pages
            builder.Add(T[MantleCmsLocalizableStrings.Pages.Title].Value, "5", item => item
                .Url("#pages")
                //.Action("Index", "Page", new { area = CmsConstants.Areas.Pages })
                .Icons("kore-icon kore-icon-pages")
                .Permission(CmsPermissions.PagesRead));

            // Queued Emails
            builder.Add(T[MantleCmsLocalizableStrings.Messaging.QueuedEmails].Value, "5", item => item
                .Url("#messaging/queued-email")
                //.Action("Index", "QueuedEmail", new { area = CmsConstants.Areas.Messaging })
                .Icons("kore-icon kore-icon-message-queue")
                .Permission(CmsPermissions.QueuedEmailsRead));

            // Subscribers
            builder.Add(T[MantleCmsLocalizableStrings.Newsletters.Subscribers].Value, "5", item => item
                .Url("#newsletters/subscribers")
                //.Action("Index", "Subscriber", new { area = CmsConstants.Areas.Newsletters })
                .Icons("kore-icon kore-icon-subscribers")
                .Permission(CmsPermissions.NewsletterRead));

            // XML Sitemap
            builder.Add(T[MantleCmsLocalizableStrings.Sitemap.XMLSitemap].Value, "5", item => item
                .Url("#sitemap/xml-sitemap")
                //.Action("Index", "XmlSitemap", new { area = CmsConstants.Areas.Sitemap })
                .Icons("kore-icon kore-icon-sitemap")
                .Permission(CmsPermissions.SitemapRead));
        }
    }
}