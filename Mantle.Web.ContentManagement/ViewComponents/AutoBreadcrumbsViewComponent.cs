using System.Collections.Generic;
using System.Linq;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Infrastructure;
using Mantle.Threading;
using Mantle.Web.ContentManagement.Areas.Admin.Blog;
using Mantle.Web.ContentManagement.Areas.Admin.Pages;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;
using Mantle.Web.Navigation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Web.ContentManagement.ViewComponents
{
    public class AutoBreadcrumbsViewComponent : ViewComponent
    {
        private readonly BlogSettings blogSettings;
        private readonly IWebWorkContext workContext;

        public AutoBreadcrumbsViewComponent(BlogSettings blogSettings, IWebWorkContext workContext)
        {
            this.blogSettings = blogSettings;
            this.workContext = workContext;
        }

        public IViewComponentResult Invoke(string templateViewName)
        {
            var breadcrumbs = new List<Breadcrumb>();

            string currentUrlSlug = Request.Url.LocalPath.TrimStart('/');

            if (currentUrlSlug == "blog")
            {
                breadcrumbs.Add(new Breadcrumb
                {
                    Text = blogSettings.PageTitle
                });
                return View(templateViewName, breadcrumbs);
            }

            var pageService = EngineContext.Current.Resolve<IPageService>();
            var pageVersionService = EngineContext.Current.Resolve<IPageVersionService>();

            int tenantId = workContext.CurrentTenant.Id;

            PageVersion currentPageVersion;
            using (var connection = pageVersionService.OpenConnection())
            {
                currentPageVersion = connection.Query()
                    .Include(x => x.Page)
                    .FirstOrDefault(x => x.TenantId == tenantId && x.Slug == currentUrlSlug);
            }

            var allPages = pageService.Find(x => x.TenantId == tenantId && x.IsEnabled);

            if (currentPageVersion != null)
            {
                var parentId = currentPageVersion.Page.ParentId;
                while (parentId != null)
                {
                    var parentPage = allPages.FirstOrDefault(x => x.Id == parentId);

                    if (parentPage == null)
                    {
                        break;
                    }

                    bool hasAccess = AsyncHelper.RunSync(() => PageSecurityHelper.CheckUserHasAccessToPage(parentPage, User));
                    if (hasAccess)
                    {
                        var currentVersion = pageVersionService.GetCurrentVersion(tenantId, parentPage.Id, workContext.CurrentCultureCode);
                        breadcrumbs.Add(new Breadcrumb
                        {
                            Text = currentVersion.Title,
                            Url = parentPage.IsEnabled ? "/" + currentVersion.Slug : null
                        });
                    }

                    parentId = parentPage.ParentId;
                }

                breadcrumbs.Reverse();

                breadcrumbs.Add(new Breadcrumb
                {
                    Text = currentPageVersion.Title
                });
            }
            else
            {
                // This is not a CMS page, so use breadcrumbs specified in controller actions...
                breadcrumbs.AddRange(workContext.Breadcrumbs);
            }

            return View(templateViewName, breadcrumbs);
        }
    }
}