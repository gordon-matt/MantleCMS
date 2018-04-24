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
                    ModuleId = "/Mantle.Web.ContentManagement.Areas.Admin.Blog.Scripts.blog",
                    Route = "blog",
                    Title = T[MantleCmsLocalizableStrings.Blog.Title].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.contentBlocks",
                    Route = "blocks/content-blocks(/:pageId)",
                    Title = T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.entityTypeContentBlocks",
                    Route = "blocks/entity-type-content-blocks/:entityType/:entityId",
                    Title = T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/Mantle.Web.ContentManagement.Areas.Admin.Media.Scripts.media",
                    Route = "media",
                    Title = T[MantleCmsLocalizableStrings.Media.Title].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/Mantle.Web.ContentManagement.Areas.Admin.Menus.Scripts.menus",
                    Route = "menus",
                    Title = T[MantleCmsLocalizableStrings.Menus.Title].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Scripts.subscribers",
                    Route = "newsletters/subscribers",
                    Title = T[MantleCmsLocalizableStrings.Newsletters.Subscribers].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/Mantle.Web.ContentManagement.Areas.Admin.Pages.Scripts.index",
                    Route = "pages",
                    Title = T[MantleCmsLocalizableStrings.Pages.Title].Value
                });

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Scripts.index",
                    Route = "sitemap/xml-sitemap",
                    Title = T[MantleCmsLocalizableStrings.Sitemap.XMLSitemap].Value
                });

                return routes;
            }
        }

        public IDictionary<string, string> ModuleIdToViewUrlMappings => new Dictionary<string, string>
        {
            { "/Mantle.Web.ContentManagement.Areas.Admin.Blog.Scripts.blog", "admin/blog" },
            { "/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.contentBlocks", "admin/blocks/content-blocks" },
            { "/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.entityTypeContentBlocks", "admin/blocks/entity-type-content-blocks" },
            { "/Mantle.Web.ContentManagement.Areas.Admin.Media.Scripts.media", "admin/media" },
            { "/Mantle.Web.ContentManagement.Areas.Admin.Menus.Scripts.menus", "admin/menus" },
            { "/Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Scripts.subscribers", "admin/newsletters/subscribers" },
            { "/Mantle.Web.ContentManagement.Areas.Admin.Pages.Scripts.index", "admin/pages" },
            { "/Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Scripts.index", "admin/xml-sitemap" },
        };

        #endregion IAureliaRouteProvider Members
    }
}