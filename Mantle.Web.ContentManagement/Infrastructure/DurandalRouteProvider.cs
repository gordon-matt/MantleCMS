using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.ContentManagement.Infrastructure
{
    public class DurandalRouteProvider : IDurandalRouteProvider
    {
        #region IDurandalRouteProvider Members

        public string Area => MantleWebConstants.Areas.Admin;

        public IEnumerable<DurandalRoute> Routes
        {
            get
            {
                var T = EngineContext.Current.Resolve<IStringLocalizer>();
                var routes = new List<DurandalRoute>();

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/blog",
                    Route = "blog",
                    JsPath = "/Mantle.Web.ContentManagement.Areas.Admin.Blog.Scripts.blog",
                    Title = T[MantleCmsLocalizableStrings.Blog.Title].Value
                });

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/blocks/content-blocks",
                    Route = "blocks/content-blocks(/:pageId)",
                    JsPath = "/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.contentBlocks",
                    Title = T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value
                });

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/blocks/entity-type-content-blocks",
                    Route = "blocks/entity-type-content-blocks/:entityType/:entityId",
                    JsPath = "/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.entityTypeContentBlocks",
                    Title = T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value
                });

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/media",
                    Route = "media",
                    JsPath = "/Mantle.Web.ContentManagement.Areas.Admin.Media.Scripts.media",
                    Title = T[MantleCmsLocalizableStrings.Media.Title].Value
                });

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/menus",
                    Route = "menus",
                    JsPath = "/Mantle.Web.ContentManagement.Areas.Admin.Menus.Scripts.menus",
                    Title = T[MantleCmsLocalizableStrings.Menus.Title].Value
                });

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/newsletters/subscribers",
                    Route = "newsletters/subscribers",
                    JsPath = "/Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Scripts.subscribers",
                    Title = T[MantleCmsLocalizableStrings.Newsletters.Subscribers].Value
                });

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/pages",
                    Route = "pages",
                    JsPath = "/Mantle.Web.ContentManagement.Areas.Admin.Pages.Scripts.index",
                    Title = T[MantleCmsLocalizableStrings.Pages.Title].Value
                });

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/sitemap/xml-sitemap",
                    Route = "sitemap/xml-sitemap",
                    JsPath = "/Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Scripts.index",
                    Title = T[MantleCmsLocalizableStrings.Sitemap.XMLSitemap].Value
                });

                return routes;
            }
        }

        #endregion IDurandalRouteProvider Members
    }
}