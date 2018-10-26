using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.ContentManagement.Infrastructure
{
    public class AureliaRouteProvider : IAureliaRouteProvider
    {
        #region IAureliaRouteProvider Members

        public string Area => MantleWebConstants.Areas.Admin;

        public IEnumerable<AureliaRoute> Routes
        {
            get
            {
                var T = EngineContext.Current.Resolve<IStringLocalizer>();
                var routes = new List<AureliaRoute>();

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Blog.Scripts.index",
                    Route = "blog",
                    Name = "mantle-cms/blog",
                    Title = T[MantleCmsLocalizableStrings.Blog.Title].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.content-blocks-index",
                    Route = "blocks/content-blocks/:pageId?",
                    Name = "mantle-cms/blocks/content-blocks",
                    Title = T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.entity-type-content-blocks-index",
                    Route = "blocks/entity-type-content-blocks/:entityType/:entityId",
                    Name = "mantle-cms/blocks/entity-type-content-blocks",
                    Title = T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Media.Scripts.media",
                    Route = "media",
                    Name = "mantle-cms/media",
                    Title = T[MantleCmsLocalizableStrings.Media.Title].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Menus.Scripts.index",
                    Route = "menus",
                    Name = "mantle-cms/menus",
                    Title = T[MantleCmsLocalizableStrings.Menus.Title].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Scripts.subscribers",
                    Route = "newsletters/subscribers",
                    Name = "mantle-cms/newsletters/subscribers",
                    Title = T[MantleCmsLocalizableStrings.Newsletters.Subscribers].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Pages.Scripts.index",
                    Route = "pages",
                    Name = "mantle-cms/pages",
                    Title = T[MantleCmsLocalizableStrings.Pages.Title].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Scripts.xml-sitemap-index",
                    Route = "sitemap/xml-sitemap",
                    Name = "mantle-cms/sitemap/xml-sitemap",
                    Title = T[MantleCmsLocalizableStrings.Sitemap.XMLSitemap].Value
                });

                #region Content Blocks

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.Blocks.html-block",
                    Route = "blocks/content-block-types/default/html-block/edit/:id",
                    Name = "mantle-cms/blocks/content-block-types/default/html-block",
                    Title = "HTML Block"
                });

                #endregion

                return routes;
            }
        }

        public IDictionary<string, string> ModuleIdToViewUrlMappings => new Dictionary<string, string>
        {
            { "aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Blog.Scripts.index", "admin/blog" },
            { "aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.content-blocks-index", "admin/blocks/content-blocks" },
            { "aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.entity-type-content-blocks-index", "admin/blocks/entity-type-content-blocks" },
            { "aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Media.Scripts.media", "admin/media" },
            { "aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Menus.Scripts.index", "admin/menus" },
            { "aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Scripts.subscribers", "admin/newsletters/subscribers" },
            { "aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Pages.Scripts.index", "admin/pages" },
            { "aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Scripts.xml-sitemap-index", "admin/sitemap/xml-sitemap" },

            // Blocks
            { "/aurelia-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.Blocks.html-block", "admin/blocks/content-block-types/default/html-block/edit" },
        };

        #endregion IAureliaRouteProvider Members
    }
}