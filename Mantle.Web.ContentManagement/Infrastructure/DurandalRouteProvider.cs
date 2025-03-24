using Mantle.Web.Infrastructure;

namespace Mantle.Web.ContentManagement.Infrastructure;

public class DurandalRouteProvider : IDurandalRouteProvider
{
    public IEnumerable<DurandalRoute> Routes
    {
        get
        {
            var T = EngineContext.Current.Resolve<IStringLocalizer>();
            var routes = new List<DurandalRoute>
            {
                new()
                {
                    ModuleId = "viewmodels/admin/blog",
                    Route = "blog",
                    JsPath = "/_content/MantleFramework.Web.ContentManagement/js/app/blog",
                    Title = T[MantleCmsLocalizableStrings.Blog.Title].Value
                },

                new()
                {
                    ModuleId = "viewmodels/admin/blocks/content-blocks",
                    Route = "blocks/content-blocks(/:pageId)",
                    JsPath = "/_content/MantleFramework.Web.ContentManagement/js/app/content-blocks",
                    Title = T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value
                },

                new()
                {
                    ModuleId = "viewmodels/admin/blocks/entity-type-content-blocks",
                    Route = "blocks/entity-type-content-blocks/:entityType/:entityId",
                    JsPath = "/_content/MantleFramework.Web.ContentManagement/js/app/entity-type-content-blocks",
                    Title = T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value
                },

                new()
                {
                    ModuleId = "viewmodels/admin/media",
                    Route = "media",
                    JsPath = "/_content/MantleFramework.Web.ContentManagement/js/app/media",
                    Title = T[MantleCmsLocalizableStrings.Media.Title].Value
                },

                new()
                {
                    ModuleId = "viewmodels/admin/menus",
                    Route = "menus",
                    JsPath = "/_content/MantleFramework.Web.ContentManagement/js/app/menus",
                    Title = T[MantleCmsLocalizableStrings.Menus.Title].Value
                },

                new()
                {
                    ModuleId = "viewmodels/admin/newsletters/subscribers",
                    Route = "newsletters/subscribers",
                    JsPath = "/_content/MantleFramework.Web.ContentManagement/js/app/subscribers",
                    Title = T[MantleCmsLocalizableStrings.Newsletters.Subscribers].Value
                },

                new()
                {
                    ModuleId = "viewmodels/admin/pages",
                    Route = "pages",
                    JsPath = "/_content/MantleFramework.Web.ContentManagement/js/app/pages",
                    Title = T[MantleCmsLocalizableStrings.Pages.Title].Value
                },

                new()
                {
                    ModuleId = "viewmodels/admin/sitemap/xml-sitemap",
                    Route = "sitemap/xml-sitemap",
                    JsPath = "/_content/MantleFramework.Web.ContentManagement/js/app/xml-sitemap",
                    Title = T[MantleCmsLocalizableStrings.Sitemap.XMLSitemap].Value
                }
            };

            return routes;
        }
    }
}