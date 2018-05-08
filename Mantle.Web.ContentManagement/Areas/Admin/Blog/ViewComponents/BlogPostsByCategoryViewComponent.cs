using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mantle.Data.Entity;
using Mantle.Data.Entity;
using Mantle.Security.Membership;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.ViewComponents
{
    [ViewComponent(Name = "BlogPostsByCategory")]
    public class BlogPostsByCategoryViewComponent : BaseBlogPostsViewComponent
    {
        private readonly IStringLocalizer T;
        private readonly IBlogCategoryService categoryService;

        public BlogPostsByCategoryViewComponent(
            Lazy<IBlogPostService> postService,
            Lazy<IBlogTagService> tagService,
            Lazy<IMembershipService> membershipService,
            BlogSettings blogSettings,
            IWorkContext workContext,
            IBlogCategoryService categoryService,
            IStringLocalizer localizer)
            : base(postService, tagService, membershipService, blogSettings, workContext)
        {
            this.categoryService = categoryService;
            T = localizer;
        }

        public async Task<IViewComponentResult> InvokeAsync(string categorySlug)
        {
            if (!await PageSecurityHelper.CheckUserHasAccessToBlog(User))
            {
                return Content("");
            }

            int tenantId = WorkContext.CurrentTenant.Id;
            var category = await categoryService.FindOneAsync(x => x.TenantId == tenantId && x.UrlSlug == categorySlug);

            if (category == null)
            {
                throw new EntityNotFoundException(string.Concat(
                    "Could not find a blog category with slug, '", categorySlug, "'"));
            }

            WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Blog.Title].Value, Url.Action("Index"));
            WorkContext.Breadcrumbs.Add(category.Name);

            string pageIndexParam = Request.Query["pageIndex"];
            int pageIndex = string.IsNullOrEmpty(pageIndexParam)
                ? 1
                : Convert.ToInt32(pageIndexParam);

            List<BlogPost> model = null;
            using (var connection = PostService.Value.OpenConnection())
            {
                model = await connection.Query()
                    .Include(x => x.Category)
                    .Include(x => x.Tags)
                    .Where(x => x.CategoryId == category.Id)
                    .OrderByDescending(x => x.DateCreatedUtc)
                    .Skip((pageIndex - 1) * BlogSettings.ItemsPerPage)
                    .Take(BlogSettings.ItemsPerPage)
                    .ToListAsync();
            }

            return await Posts(pageIndex, model);
        }
    }
}