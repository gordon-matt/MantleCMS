using Extenso.AspNetCore.OData;
using Mantle.Localization;
using Mantle.Web.Configuration;
using Mantle.Web.ContentManagement.Areas.Admin.Blog;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.ContentBlocks;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Localization;
using Mantle.Web.ContentManagement.Areas.Admin.Media.ContentBlocks;
using Mantle.Web.ContentManagement.Areas.Admin.Menus.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Newsletters;
using Mantle.Web.ContentManagement.Areas.Admin.Newsletters.ContentBlocks;
using Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Pages;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;
using Mantle.Web.Infrastructure;
using Mantle.Web.Navigation;
using Mantle.Web.Security.Membership;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.ContentManagement.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(IContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        //builder.Register<IDbSeeder, DbSeeder>(ServiceLifetime.Transient); // TODO?

        builder.Register<IDurandalRouteProvider, DurandalRouteProvider>(ServiceLifetime.Singleton);

        // Embedded File Provider
        builder.Register<IEmbeddedFileProviderRegistrar, EmbeddedFileProviderRegistrar>(ServiceLifetime.Scoped);

        #region Services

        // Blog
        builder.Register<IBlogCategoryService, BlogCategoryService>(ServiceLifetime.Transient);
        builder.Register<IBlogPostService, BlogPostService>(ServiceLifetime.Transient);
        builder.Register<IBlogTagService, BlogTagService>(ServiceLifetime.Transient);
        builder.Register<IBlogPostTagService, BlogPostTagService>(ServiceLifetime.Transient);

        // Menus
        builder.Register<IMenuService, MenuService>(ServiceLifetime.Transient);
        builder.Register<IMenuItemService, MenuItemService>(ServiceLifetime.Transient);

        // Pages
        builder.Register<IPageService, PageService>(ServiceLifetime.Transient);
        builder.Register<IPageTypeService, PageTypeService>(ServiceLifetime.Transient);
        builder.Register<IPageVersionService, PageVersionService>(ServiceLifetime.Transient);

        // Content Blocks
        builder.Register<IEntityTypeContentBlockService, EntityTypeContentBlockService>(ServiceLifetime.Transient);
        builder.Register<IContentBlockService, ContentBlockService>(ServiceLifetime.Transient);
        builder.Register<IZoneService, ZoneService>(ServiceLifetime.Transient);

        builder.Register<INewsletterService, NewsletterService>(ServiceLifetime.Transient);

        #endregion Services

        #region Localization

        builder.Register<ILanguagePack, LanguagePackInvariant>(ServiceLifetime.Singleton);

        #endregion Localization

        #region Navigation

        builder.Register<INavigationProvider, CmsNavigationProvider>(ServiceLifetime.Singleton);

        #endregion Navigation

        #region Security

        // Permissions
        builder.Register<IPermissionProvider, CmsPermissions>(ServiceLifetime.Singleton);

        // User Profile Providers
        builder.Register<IUserProfileProvider, NewsletterUserProfileProvider>(ServiceLifetime.Singleton);

        #endregion Security

        #region Themes

        builder.Register<ILocationFormatProvider, LocationFormatProvider>(ServiceLifetime.Singleton);

        #endregion Themes

        #region Configuration

        builder.Register<ISettings, BlogSettings>(ServiceLifetime.Scoped);
        builder.Register<ISettings, PageSettings>(ServiceLifetime.Scoped);

        #endregion Configuration

        #region Content Blocks

        // Blogs
        builder.Register<IContentBlock, FilteredPostsBlock>(ServiceLifetime.Transient);
        builder.Register<IContentBlock, LastNPostsBlock>(ServiceLifetime.Transient);
        builder.Register<IContentBlock, TagCloudBlock>(ServiceLifetime.Transient);
        builder.Register<IContentBlock, CategoriesBlock>(ServiceLifetime.Transient);

        // Other
        builder.Register<IContentBlock, FormBlock>(ServiceLifetime.Transient);
        builder.Register<IContentBlock, HtmlBlock>(ServiceLifetime.Transient);
        builder.Register<IContentBlock, LanguageSwitchBlock>(ServiceLifetime.Transient);
        builder.Register<IContentBlock, NewsletterSubscriptionBlock>(ServiceLifetime.Transient);
        builder.Register<IContentBlock, VideoBlock>(ServiceLifetime.Transient);

        #endregion Content Blocks

        #region Other: Content Blocks

        builder.Register<IContentBlockProvider, DefaultContentBlockProvider>(ServiceLifetime.Transient);
        builder.Register<IEntityTypeContentBlockProvider, DefaultEntityTypeContentBlockProvider>(ServiceLifetime.Transient);

        #endregion Other: Content Blocks

        // Other
        //builder.Register<IResourceBundleRegistrar, ResourceBundleRegistrar>(ServiceLifetime.Singleton); // TODO?
        //builder.Register<IRequireJSConfigProvider, RequireJSConfigProvider>(ServiceLifetime.Singleton);
        builder.Register<IODataRegistrar, ODataRegistrar>(ServiceLifetime.Singleton);

        // Indexing
        //builder.Register<IIndexingContentProvider, PagesIndexingContentProvider>(ServiceLifetime.Transient); // TODO
        //builder.Register<IIndexingContentProvider, BlogIndexingContentProvider>(ServiceLifetime.Transient); // TODO

        //builder.Register<IMessageTemplatesProvider, NewsletterMessageTemplates>(ServiceLifetime.Transient);
    }

    public int Order => 1;

    #endregion IDependencyRegistrar Members
}