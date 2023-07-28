using Mantle.Web.Navigation;
using NavigationBuilder = Mantle.Web.Navigation.NavigationBuilder;

namespace Mantle.Web.ContentManagement;

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
        builder.Icons("fa fa-edit");

        // Blog
        builder.Add(T[MantleCmsLocalizableStrings.Blog.Title].Value, "5", item => item
            .Url("#blog")
            .Icons("fa fa-bullhorn")
            .Permission(CmsPermissions.BlogRead));

        // Content Blocks
        builder.Add(T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value, "5", item => item
            .Url("#blocks/content-blocks")
            .Icons("fa fa-th-large")
            .Permission(CmsPermissions.ContentBlocksRead));

        // Media
        builder.Add(T[MantleCmsLocalizableStrings.Media.Title].Value, "5", item => item
            .Url("#media")
            .Icons("fa fa-picture-o")
            .Permission(CmsPermissions.MediaRead));

        // Menus
        builder.Add(T[MantleCmsLocalizableStrings.Menus.Title].Value, "5", item => item
            .Url("#menus")
            .Icons("fa fa-arrow-right")
            .Permission(CmsPermissions.MenusRead));

        // Pages
        builder.Add(T[MantleCmsLocalizableStrings.Pages.Title].Value, "5", item => item
            .Url("#pages")
            .Icons("fa fa-file-o")
            .Permission(CmsPermissions.PagesRead));

        // Subscribers
        builder.Add(T[MantleCmsLocalizableStrings.Newsletters.Subscribers].Value, "5", item => item
            .Url("#newsletters/subscribers")
            .Icons("fa fa-users")
            .Permission(CmsPermissions.NewsletterRead));

        // XML Sitemap
        builder.Add(T[MantleCmsLocalizableStrings.Sitemap.XMLSitemap].Value, "5", item => item
            .Url("#sitemap/xml-sitemap")
            .Icons("fa fa-sitemap")
            .Permission(CmsPermissions.SitemapRead));
    }
}