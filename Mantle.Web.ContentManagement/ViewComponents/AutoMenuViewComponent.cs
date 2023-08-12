using Mantle.Web.ContentManagement.Areas.Admin.Blog;
using Mantle.Web.ContentManagement.Areas.Admin.Menus.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Pages;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;
using Mantle.Web.ContentManagement.Infrastructure;

namespace Mantle.Web.ContentManagement.ViewComponents;

[ViewComponent(Name = "AutoMenu")]
public class AutoMenuViewComponent : ViewComponent
{
    private readonly IPageService pageService;
    private readonly IPageVersionService pageVersionService;
    private readonly IEnumerable<IAutoMenuProvider> menuProviders;
    private readonly BlogSettings blogSettings;
    private readonly IStringLocalizer T;
    private readonly IWorkContext workContext;

    public AutoMenuViewComponent(
        IPageService pageService,
        IPageVersionService pageVersionService,
        IEnumerable<IAutoMenuProvider> menuProviders,
        BlogSettings blogSettings,
        IStringLocalizer stringLocalizer,
        IWorkContext workContext)
    {
        this.pageService = pageService;
        this.pageVersionService = pageVersionService;
        this.menuProviders = menuProviders;
        this.blogSettings = blogSettings;
        this.T = stringLocalizer;
        this.workContext = workContext;
    }

    public async Task<IViewComponentResult> InvokeAsync(string templateViewName, bool includeHomePageLink = true)
    {
        var menuItems = new List<MenuItem>();
        var menuId = Guid.NewGuid();

        if (includeHomePageLink)
        {
            menuItems.Add(new MenuItem
            {
                Id = menuId,
                Text = T[MantleWebLocalizableStrings.General.Home],
                Url = "/",
                Enabled = true,
                ParentId = null,
                Position = -1 // Always first
            });
        }

        int tenantId = workContext.CurrentTenant.Id;

        var pageVersions = pageVersionService.GetCurrentVersions(
            tenantId,
            workContext.CurrentCultureCode,
            enabledOnly: true,
            shownOnMenusOnly: true);

        var authorizedPages = pageVersions.Where(x => AsyncHelper.RunSync(() => PageSecurityHelper.CheckUserHasAccessToPage(x.Page, User)));

        var items = authorizedPages
            .Select(x => new MenuItem
            {
                Id = x.Page.Id,
                Text = x.Title,
                Url = "/" + x.Slug,
                Enabled = true,
                ParentId = x.Page.ParentId,
                Position = x.Page.Order
            });

        menuItems.AddRange(items);

        bool hasAccess = await PageSecurityHelper.CheckUserHasAccessToBlog(User);
        if (hasAccess && blogSettings.ShowOnMenus)
        {
            menuItems.Add(new MenuItem
            {
                Id = menuId,
                Text = blogSettings.PageTitle,
                Url = "/blog",
                Enabled = true,
                ParentId = null,
                Position = blogSettings.MenuPosition
            });
        }

        foreach (var menuProvider in menuProviders)
        {
            menuItems.AddRange(menuProvider.GetMainMenuItems(User));
        }

        menuItems = menuItems
            .OrderBy(x => x.Position)
            .ThenBy(x => x.Text)
            .ToList();

        ViewBag.MenuId = menuId;
        ViewBag.TemplateViewName = templateViewName;

        return View(menuItems);
    }
}