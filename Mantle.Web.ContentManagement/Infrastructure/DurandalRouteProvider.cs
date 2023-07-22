using Mantle.Infrastructure;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.ContentManagement.Infrastructure
{
    public class DurandalRouteProvider : IDurandalRouteProvider
    {
        public IEnumerable<DurandalRoute> Routes
        {
            get
            {
                var T = EngineContext.Current.Resolve<IStringLocalizer>();
                var routes = new List<DurandalRoute>
                {
                    new DurandalRoute
                    {
                        ModuleId = "viewmodels/admin/blog",
                        Route = "blog",
                        JsPath = "/durandal-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Blog.Scripts.blog",
                        Title = T[MantleCmsLocalizableStrings.Blog.Title].Value
                    },

                    new DurandalRoute
                    {
                        ModuleId = "viewmodels/admin/blocks/content-blocks",
                        Route = "blocks/content-blocks(/:pageId)",
                        JsPath = "/durandal-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.content-blocks-index",
                        Title = T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value
                    },

                    new DurandalRoute
                    {
                        ModuleId = "viewmodels/admin/blocks/entity-type-content-blocks",
                        Route = "blocks/entity-type-content-blocks/:entityType/:entityId",
                        JsPath = "/durandal-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.entity-type-content-blocks-index",
                        Title = T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value
                    },

                    new DurandalRoute
                    {
                        ModuleId = "viewmodels/admin/media",
                        Route = "media",
                        JsPath = "/durandal-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Media.Scripts.media",
                        Title = T[MantleCmsLocalizableStrings.Media.Title].Value
                    },

                    new DurandalRoute
                    {
                        ModuleId = "viewmodels/admin/menus",
                        Route = "menus",
                        JsPath = "/durandal-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Menus.Scripts.index",
                        Title = T[MantleCmsLocalizableStrings.Menus.Title].Value
                    },

                    new DurandalRoute
                    {
                        ModuleId = "viewmodels/admin/newsletters/subscribers",
                        Route = "newsletters/subscribers",
                        JsPath = "/durandal-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Scripts.subscribers",
                        Title = T[MantleCmsLocalizableStrings.Newsletters.Subscribers].Value
                    },

                    new DurandalRoute
                    {
                        ModuleId = "viewmodels/admin/pages",
                        Route = "pages",
                        JsPath = "/durandal-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Pages.Scripts.index",
                        Title = T[MantleCmsLocalizableStrings.Pages.Title].Value
                    },

                    new DurandalRoute
                    {
                        ModuleId = "viewmodels/admin/sitemap/xml-sitemap",
                        Route = "sitemap/xml-sitemap",
                        JsPath = "/durandal-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Scripts.xml-sitemap-index",
                        Title = T[MantleCmsLocalizableStrings.Sitemap.XMLSitemap].Value
                    }
                };

                #region Content Blocks

                //routes.Add(new DurandalRoute
                //{
                //    ModuleId = "/durandal-app/embedded/Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripts.Blocks.html-block",
                //    Route = "blocks/content-block-types/default/html-block/edit/:id",
                //    Name = "mantle-cms/blocks/content-block-types/default/html-block",
                //    Title = "HTML Block"
                //});

                #endregion Content Blocks

                return routes;
            }
        }
    }
}