//using System.Web.Optimization;
//using Mantle.Web.Infrastructure;

//namespace Mantle.Web.ContentManagement.Infrastructure
//{
//    public class ResourceBundleRegistrar : IResourceBundleRegistrar
//    {
//        #region IResourceBundleRegistrar Members

//        public void Register(BundleCollection bundles)
//        {
//            #region Scripts

//            #region 3rd Party

//            bundles.Add(new ScriptBundle("~/bundles/js/third-party/jQCloud")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Scripts.jqcloud.min.js"));

//            #endregion

//            bundles.Add(new ScriptBundle("~/bundles/js/mantle-cms/blog")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Areas.Admin.Blog.Scripts.blog.js"));

//            bundles.Add(new ScriptBundle("~/bundles/js/mantle-cms/blog-content")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Scripts.jquery.bootpag.min.js")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Scripts.moment.js")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Areas.Admin.Blog.Scripts.blogContent.js"));

//            bundles.Add(new ScriptBundle("~/bundles/js/mantle-cms/content-blocks")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.contentBlocks.js"));

//            bundles.Add(new ScriptBundle("~/bundles/js/mantle-cms/entity-type-content-blocks")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.entityTypeContentBlocks.js"));

//            bundles.Add(new ScriptBundle("~/bundles/js/mantle-cms/languages")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Areas.Admin.Localization.Scripts.index.js"));

//            bundles.Add(new ScriptBundle("~/bundles/js/mantle-cms/localizable-strings")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Areas.Admin.Localization.Scripts.localizableStrings.js"));

//            bundles.Add(new ScriptBundle("~/bundles/js/mantle-cms/media")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Areas.Admin.Media.Scripts.media.js"));

//            bundles.Add(new ScriptBundle("~/bundles/js/mantle-cms/menus")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Areas.Admin.Menus.Scripts.menus.js"));

//            bundles.Add(new ScriptBundle("~/bundles/js/mantle-cms/message-templates")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Areas.Admin.Messaging.Scripts.messageTemplates.js"));

//            bundles.Add(new ScriptBundle("~/bundles/js/mantle-cms/newsletters/subscribers")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Scripts.subscribers.js"));

//            bundles.Add(new ScriptBundle("~/bundles/js/mantle-cms/pages")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Areas.Admin.Pages.Scripts.index.js"));

//            bundles.Add(new ScriptBundle("~/bundles/js/mantle-cms/queued-emails")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Areas.Admin.Messaging.Scripts.queuedEmails.js"));

//            bundles.Add(new ScriptBundle("~/bundles/js/mantle-cms/xml-sitemap")
//                .Include("~/Scripts/Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Scripts.index.js"));

//            #endregion

//            #region Styles

//            IItemTransform cssRewriteUrlTransform = new CssRewriteUrlTransform();

//            bundles.Add(new StyleBundle("~/bundles/content/third-party/jQCloud")
//                .Include("~/Content/Mantle.Web.ContentManagement.Content.jqcloud.min.css", cssRewriteUrlTransform));

//            #endregion
//        }

//        #endregion IResourceBundleRegistrar Members
//    }
//}