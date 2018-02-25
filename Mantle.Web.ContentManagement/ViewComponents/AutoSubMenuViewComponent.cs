using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mantle.Threading;
using Mantle.Web.ContentManagement.Areas.Admin.Blog;
using Mantle.Web.ContentManagement.Areas.Admin.Menus.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Pages;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;
using Mantle.Web.ContentManagement.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Web.ContentManagement.ViewComponents
{
    [ViewComponent(Name = "AutoSubMenu")]
    public class AutoSubMenuViewComponent : ViewComponent
    {
        private readonly IPageVersionService pageVersionService;
        private readonly IEnumerable<IAutoMenuProvider> menuProviders;
        private readonly BlogSettings blogSettings;
        private readonly IWebWorkContext workContext;

        public AutoSubMenuViewComponent(
            IPageVersionService pageVersionService,
            IEnumerable<IAutoMenuProvider> menuProviders,
            BlogSettings blogSettings,
            IWebWorkContext workContext)
        {
            this.pageVersionService = pageVersionService;
            this.menuProviders = menuProviders;
            this.blogSettings = blogSettings;
            this.workContext = workContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string templateViewName)
        {
            // we need a better way to get slug, because it could be something like /store/categories/category-1/product-1
            // and this current way would only return product-1
            string currentUrlSlug = Request.Path.Value.TrimStart('/'); // TODO: Test
            var menuItems = new List<MenuItem>();
            var menuId = Guid.NewGuid();

            // If home page
            if (string.IsNullOrEmpty(currentUrlSlug))
            {
                bool hasAccess = await PageSecurityHelper.CheckUserHasAccessToBlog(User);
                if (blogSettings.ShowOnMenus && hasAccess)
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
                    menuItems.AddRange(menuProvider.GetMainMenuItems(User).Where(x => x.ParentId == null));
                }
            }
            foreach (var menuProvider in menuProviders)
            {
                if (currentUrlSlug.StartsWith(menuProvider.RootUrlSlug))
                {
                    menuItems.AddRange(menuProvider.GetSubMenuItems(currentUrlSlug, User));
                }
            }

            var pageVersions = Enumerable.Empty<PageVersion>();

            bool hasCmsPages = true;

            int tenantId = workContext.CurrentTenant.Id;

            // If on home page
            if (string.IsNullOrEmpty(currentUrlSlug))
            {
                pageVersions = pageVersionService.GetCurrentVersions(
                    tenantId,
                    workContext.CurrentCultureCode,
                    enabledOnly: true,
                    shownOnMenusOnly: true,
                    topLevelOnly: true);
            }
            else
            {
                // We don't care about culture here because the only thing we're interested in getting is
                //  the Page ID, which will of course be the same for all versions of a page.
                PageVersion anyVersion = null;
                using (var connection = pageVersionService.OpenConnection())
                {
                    anyVersion = connection.Query(x =>
                            x.TenantId == tenantId &&
                            x.Slug == currentUrlSlug)
                        .Include(x => x.Page)
                        .FirstOrDefault();
                }

                // If the current page is a CMS page
                if (anyVersion != null)
                {
                    pageVersions = pageVersionService.GetCurrentVersions(
                        tenantId,
                        workContext.CurrentCultureCode,
                        enabledOnly: true,
                        shownOnMenusOnly: true,
                        parentId: anyVersion.Page.Id);
                }
                else
                {
                    hasCmsPages = false;
                }
            }

            if (hasCmsPages)
            {
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
}