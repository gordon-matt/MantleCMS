using Mantle.Web.ContentManagement.Areas.Admin.Menus.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuService service;
        private readonly IMenuItemService menuItemService;
        private readonly IPageVersionService pageVersionService;
        private readonly IWebWorkContext workContext;

        public MenuViewComponent(IMenuService service, IMenuItemService menuItemService, IPageVersionService pageVersionService, IWebWorkContext workContext)
        {
            this.service = service;
            this.menuItemService = menuItemService;
            this.pageVersionService = pageVersionService;
            this.workContext = workContext;
        }

        public IViewComponentResult Invoke(string name, string templateViewName, bool filterByUrl = false)
        {
            string currentUrlSlug = filterByUrl ? Request.Url.LocalPath.TrimStart('/') : null;

            // Check if it's a CMS page or not.
            int tenantId = workContext.CurrentTenant.Id;
            if (currentUrlSlug != null && pageVersionService.Find(x => x.TenantId == tenantId && x.Slug == currentUrlSlug) == null)
            {
                // It's not a CMS page, so don't try to filter by slug...
                // Set slug to null, to query for a menu without any URL filter
                currentUrlSlug = null;
            }

            var menu = service.FindByName(workContext.CurrentTenant.Id, name, currentUrlSlug);

            if (menu == null)
            {
                return Content(string.Empty);
                //throw new ArgumentException("There is no menu named, '" + name + "'");
            }

            var menuItems = menuItemService.GetMenuItems(menu.Id, true);

            ViewBag.MenuId = menu.Id;
            return View(templateViewName, menuItems);
        }
    }
}