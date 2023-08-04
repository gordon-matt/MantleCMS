using Autofac;
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

namespace Mantle.Web.ContentManagement.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
    {
        //builder.RegisterType<DbSeeder>().As<IDbSeeder>().InstancePerDependency(); // TODO?

        builder.RegisterType<DurandalRouteProvider>().As<IDurandalRouteProvider>().SingleInstance();

        // Embedded File Provider
        builder.RegisterType<EmbeddedFileProviderRegistrar>().As<IEmbeddedFileProviderRegistrar>().InstancePerLifetimeScope();

        #region Services

        // Blog
        builder.RegisterType<BlogCategoryService>().As<IBlogCategoryService>().InstancePerDependency();
        builder.RegisterType<BlogPostService>().As<IBlogPostService>().InstancePerDependency();
        builder.RegisterType<BlogTagService>().As<IBlogTagService>().InstancePerDependency();
        builder.RegisterType<BlogPostTagService>().As<IBlogPostTagService>().InstancePerDependency();

        // Menus
        builder.RegisterType<MenuService>().As<IMenuService>().InstancePerDependency();
        builder.RegisterType<MenuItemService>().As<IMenuItemService>().InstancePerDependency();

        // Pages
        builder.RegisterType<PageService>().As<IPageService>().InstancePerDependency();
        builder.RegisterType<PageTypeService>().As<IPageTypeService>().InstancePerDependency();
        builder.RegisterType<PageVersionService>().As<IPageVersionService>().InstancePerDependency();

        // Content Blocks
        builder.RegisterType<EntityTypeContentBlockService>().As<IEntityTypeContentBlockService>().InstancePerDependency();
        builder.RegisterType<ContentBlockService>().As<IContentBlockService>().InstancePerDependency();
        builder.RegisterType<ZoneService>().As<IZoneService>().InstancePerDependency();

        builder.RegisterType<NewsletterService>().As<INewsletterService>().InstancePerDependency();

        #endregion Services

        #region Localization

        builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();

        #endregion Localization

        #region Navigation

        builder.RegisterType<CmsNavigationProvider>().As<INavigationProvider>().SingleInstance();

        #endregion Navigation

        #region Security

        // Permissions
        builder.RegisterType<CmsPermissions>().As<IPermissionProvider>().SingleInstance();

        // User Profile Providers
        builder.RegisterType<NewsletterUserProfileProvider>().As<IUserProfileProvider>().SingleInstance();

        #endregion Security

        #region Themes

        builder.RegisterType<LocationFormatProvider>().As<ILocationFormatProvider>().SingleInstance();

        #endregion Themes

        #region Configuration

        builder.RegisterType<BlogSettings>().As<ISettings>().InstancePerLifetimeScope();
        builder.RegisterType<PageSettings>().As<ISettings>().InstancePerLifetimeScope();

        #endregion Configuration

        #region Content Blocks

        // Blogs
        builder.RegisterType<FilteredPostsBlock>().As<IContentBlock>().InstancePerDependency();
        builder.RegisterType<LastNPostsBlock>().As<IContentBlock>().InstancePerDependency();
        builder.RegisterType<TagCloudBlock>().As<IContentBlock>().InstancePerDependency();
        builder.RegisterType<CategoriesBlock>().As<IContentBlock>().InstancePerDependency();

        // Other
        builder.RegisterType<FormBlock>().As<IContentBlock>().InstancePerDependency();
        builder.RegisterType<HtmlBlock>().As<IContentBlock>().InstancePerDependency();
        builder.RegisterType<LanguageSwitchBlock>().As<IContentBlock>().InstancePerDependency();
        builder.RegisterType<NewsletterSubscriptionBlock>().As<IContentBlock>().InstancePerDependency();
        builder.RegisterType<VideoBlock>().As<IContentBlock>().InstancePerDependency();

        #endregion Content Blocks

        #region Other: Content Blocks

        builder.RegisterType<DefaultContentBlockProvider>().As<IContentBlockProvider>().InstancePerDependency();
        builder.RegisterType<DefaultEntityTypeContentBlockProvider>().As<IEntityTypeContentBlockProvider>().InstancePerDependency();

        #endregion Other: Content Blocks

        // Other
        //builder.RegisterType<ResourceBundleRegistrar>().As<IResourceBundleRegistrar>().SingleInstance(); // TODO?
        //builder.RegisterType<RequireJSConfigProvider>().As<IRequireJSConfigProvider>().SingleInstance();
        builder.RegisterType<ODataRegistrar>().As<IODataRegistrar>().SingleInstance();

        // Indexing
        //builder.RegisterType<PagesIndexingContentProvider>().As<IIndexingContentProvider>().InstancePerDependency(); // TODO
        //builder.RegisterType<BlogIndexingContentProvider>().As<IIndexingContentProvider>().InstancePerDependency(); // TODO

        //builder.RegisterType<NewsletterMessageTemplates>().As<IMessageTemplatesProvider>().InstancePerDependency();
    }

    public int Order => 1;

    #endregion IDependencyRegistrar Members
}