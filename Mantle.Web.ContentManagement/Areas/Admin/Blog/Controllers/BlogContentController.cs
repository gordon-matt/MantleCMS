using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mantle.Data.Entity;
using Mantle.Exceptions;
using Mantle.Security.Membership;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Pages;
using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Controllers
{
    [Area("")]
    [Route("blog")]
    public class BlogContentController : MantleController
    {
        private readonly BlogSettings blogSettings;
        private readonly IRazorViewEngine razorViewEngine;
        private readonly Lazy<IBlogPostService> postService;
        private readonly Lazy<IBlogCategoryService> categoryService;
        private readonly Lazy<IBlogTagService> tagService;
        private readonly Lazy<IMembershipService> membershipService;

        public BlogContentController(
            BlogSettings blogSettings,
            IRazorViewEngine razorViewEngine,
            Lazy<IBlogPostService> postService,
            Lazy<IBlogCategoryService> categoryService,
            Lazy<IBlogTagService> tagService,
            Lazy<IMembershipService> membershipService)
        {
            this.blogSettings = blogSettings;
            this.categoryService = categoryService;
            this.postService = postService;
            this.tagService = tagService;
            this.membershipService = membershipService;
            this.razorViewEngine = razorViewEngine;
        }

        [Route("{isPartialRequest}")]
        public async Task<ActionResult> Index(bool isPartialRequest)
        {
            // If there are access restrictions
            if (!await PageSecurityHelper.CheckUserHasAccessToBlog(User))
            {
                return Unauthorized();
            }

            WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Blog.Title].Value);

            if (blogSettings.UseAjax)
            {
                return PostsAjax(isPartialRequest);
            }
            else
            {
                int tenantId = WorkContext.CurrentTenant.Id;

                string pageIndexParam = Request.Query["pageIndex"];
                int pageIndex = string.IsNullOrEmpty(pageIndexParam)
                    ? 1
                    : Convert.ToInt32(pageIndexParam);

                List<BlogPost> model = null;
                using (var connection = postService.Value.OpenConnection())
                {
                    model = await connection.Query(x => x.TenantId == tenantId)
                        .Include(x => x.Category)
                        .Include(x => x.Tags)
                        .OrderByDescending(x => x.DateCreatedUtc)
                        .Skip((pageIndex - 1) * blogSettings.ItemsPerPage)
                        .Take(blogSettings.ItemsPerPage)
                        .ToListAsync();
                }

                return await Posts(pageIndex, model, isPartialRequest);
            }
        }

        [Route("category/{categorySlug}/{isPartialRequest}")]
        public async Task<ActionResult> Category(string categorySlug, bool isPartialRequest)
        {
            // If there are access restrictions
            if (!await PageSecurityHelper.CheckUserHasAccessToBlog(User))
            {
                return Unauthorized();
            }

            int tenantId = WorkContext.CurrentTenant.Id;
            var category = await categoryService.Value.FindOneAsync(x => x.TenantId == tenantId && x.UrlSlug == categorySlug);

            if (category == null)
            {
                throw new EntityNotFoundException(string.Concat(
                    "Could not find a blog category with slug, '", categorySlug, "'"));
            }

            WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Blog.Title].Value, Url.Action("Index"));
            WorkContext.Breadcrumbs.Add(category.Name);

            if (blogSettings.UseAjax)
            {
                ViewBag.CategoryId = category.Id;
                return PostsAjax(isPartialRequest);
            }
            else
            {
                string pageIndexParam = Request.Query["pageIndex"];
                int pageIndex = string.IsNullOrEmpty(pageIndexParam)
                    ? 1
                    : Convert.ToInt32(pageIndexParam);

                List<BlogPost> model = null;
                using (var connection = postService.Value.OpenConnection())
                {
                    model = await connection.Query()
                        .Include(x => x.Category)
                        .Include(x => x.Tags)
                        .Where(x => x.CategoryId == category.Id)
                        .OrderByDescending(x => x.DateCreatedUtc)
                        .Skip((pageIndex - 1) * blogSettings.ItemsPerPage)
                        .Take(blogSettings.ItemsPerPage)
                        .ToListAsync();
                }

                return await Posts(pageIndex, model, isPartialRequest);
            }
        }

        [Route("tag/{tagSlug}/{isPartialRequest}")]
        public async Task<ActionResult> Tag(string tagSlug, bool isPartialRequest)
        {
            int tenantId = WorkContext.CurrentTenant.Id;

            var tag = await tagService.Value.FindOneAsync(x =>
                x.TenantId == tenantId
                && x.UrlSlug == tagSlug);

            if (tag == null)
            {
                throw new EntityNotFoundException(string.Concat(
                    "Could not find a blog tag with slug, '", tagSlug, "'"));
            }

            WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Blog.Title].Value, Url.Action("Index"));
            WorkContext.Breadcrumbs.Add(tag.Name);

            if (blogSettings.UseAjax)
            {
                ViewBag.TagId = tag.Id;
                return PostsAjax(isPartialRequest);
            }
            else
            {
                string pageIndexParam = Request.Query["pageIndex"];
                int pageIndex = string.IsNullOrEmpty(pageIndexParam)
                    ? 1
                    : Convert.ToInt32(pageIndexParam);

                List<BlogPost> model = null;
                using (var connection = postService.Value.OpenConnection())
                {
                    model = await connection.Query()
                        .Include(x => x.Category)
                        .Include(x => x.Tags)
                        .Where(x => x.Tags.Any(y => y.TagId == tag.Id))
                        .OrderByDescending(x => x.DateCreatedUtc)
                        .Skip((pageIndex - 1) * blogSettings.ItemsPerPage)
                        .Take(blogSettings.ItemsPerPage)
                        .ToListAsync();
                }

                return await Posts(pageIndex, model, isPartialRequest);
            }
        }

        private async Task<ActionResult> Posts(int pageIndex, IEnumerable<BlogPost> model, bool isPartialRequest)
        {
            ViewBag.IsPartialRequest = isPartialRequest;

            var userIds = model.Select(x => x.UserId).Distinct();
            var userNames = (await membershipService.Value.GetUsers(WorkContext.CurrentTenant.Id, x => userIds.Contains(x.Id))).ToDictionary(k => k.Id, v => v.UserName);

            int total = await postService.Value.CountAsync();

            ViewBag.PageCount = (int)Math.Ceiling((double)total / blogSettings.ItemsPerPage);
            ViewBag.PageIndex = pageIndex;
            ViewBag.UserNames = userNames;

            var tags = await tagService.Value.FindAsync();
            ViewBag.Tags = tags.ToDictionary(k => k.Id, v => v.Name);
            ViewBag.TagUrls = tags.ToDictionary(k => k.Id, v => v.UrlSlug);

            var viewEngineResult = razorViewEngine.FindView(ControllerContext, "Index", false);

            // If someone has provided a custom template (see LocationFormatProvider)
            if (viewEngineResult.View != null)
            {
                if (isPartialRequest)
                {
                    return PartialView("Index", model);
                }
                return View("Index", model);
            }

            // Else use default template
            if (isPartialRequest)
            {
                return PartialView("Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.BlogContent.Index", model);
            }
            return View("Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.BlogContent.Index", model);
        }

        private ActionResult PostsAjax(bool isPartialRequest)
        {
            ViewBag.IsPartialRequest = isPartialRequest;

            var viewEngineResult = razorViewEngine.FindView(ControllerContext, "IndexAjax", false);

            // If someone has provided a custom template (see LocationFormatProvider)
            if (viewEngineResult.View != null)
            {
                if (isPartialRequest)
                {
                    return PartialView("IndexAjax");
                }
                return View("IndexAjax");
            }

            // Else use default template
            if (isPartialRequest)
            {
                return PartialView("Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.BlogContent.IndexAjax");
            }
            return View("Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.BlogContent.IndexAjax");
        }

        [Route("{slug}")]
        public async Task<ActionResult> Details(string slug)
        {
            // If there are access restrictions
            if (!await PageSecurityHelper.CheckUserHasAccessToBlog(User))
            {
                return Unauthorized();
            }

            int tenantId = WorkContext.CurrentTenant.Id;

            BlogPost model = null;
            DateTime? previousEntryDate = null;
            DateTime? nextEntryDate = null;
            using (var connection = postService.Value.OpenConnection())
            {
                model = await connection
                    .Query(x => x.TenantId == tenantId && x.Slug == slug)
                    .Include(x => x.Category)
                    .Include(x => x.Tags)
                    .FirstOrDefaultAsync();

                if (model == null)
                {
                    throw new MantleException("Blog post not found!");
                }

                bool hasPreviousEntry = await connection.Query(x => x.TenantId == tenantId).AnyAsync(x => x.DateCreatedUtc < model.DateCreatedUtc);
                if (hasPreviousEntry)
                {
                    previousEntryDate = await connection.Query(x => x.TenantId == tenantId && x.DateCreatedUtc < model.DateCreatedUtc)
                        .Select(x => x.DateCreatedUtc)
                        .MaxAsync();
                }

                bool hasNextEntry = await connection.Query(x => x.TenantId == tenantId).AnyAsync(x => x.DateCreatedUtc > model.DateCreatedUtc);
                if (hasNextEntry)
                {
                    nextEntryDate = await connection.Query(x => x.TenantId == tenantId && x.DateCreatedUtc > model.DateCreatedUtc)
                        .Select(x => x.DateCreatedUtc)
                        .MinAsync();
                }
            }

            WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Blog.Title].Value, Url.Action("Index"));
            WorkContext.Breadcrumbs.Add(model.Headline);

            ViewBag.PreviousEntrySlug = previousEntryDate.HasValue
                ? (await postService.Value.FindOneAsync(x => x.TenantId == tenantId && x.DateCreatedUtc == previousEntryDate)).Slug
                : null;

            ViewBag.NextEntrySlug = nextEntryDate.HasValue
                ? (await postService.Value.FindOneAsync(x => x.TenantId == tenantId && x.DateCreatedUtc == nextEntryDate)).Slug
                : null;

            ViewBag.UserName = (await membershipService.Value.GetUserById(model.UserId)).UserName;

            var tags = await tagService.Value.FindAsync(x => x.TenantId == tenantId);
            ViewBag.Tags = tags.ToDictionary(k => k.Id, v => v.Name);
            ViewBag.TagUrls = tags.ToDictionary(k => k.Id, v => v.UrlSlug);

            var viewEngineResult = razorViewEngine.FindView(ControllerContext, "Details", false);

            // If someone has provided a custom template (see LocationFormatProvider)
            if (viewEngineResult.View != null)
            {
                return View(model);
            }

            // Else use default template
            return View("Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.BlogContent.Details", model);
        }
    }
}