using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mantle.Security.Membership;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.ViewComponents
{
    public abstract class BaseBlogPostsViewComponent : ViewComponent
    {
        protected readonly Lazy<IBlogPostService> PostService;
        protected readonly Lazy<IBlogTagService> TagService;
        protected readonly Lazy<IMembershipService> MembershipService;
        protected readonly BlogSettings BlogSettings;
        protected readonly IWorkContext WorkContext;

        public BaseBlogPostsViewComponent(
            Lazy<IBlogPostService> postService,
            Lazy<IBlogTagService> tagService,
            Lazy<IMembershipService> membershipService,
            BlogSettings blogSettings,
            IWorkContext workContext)
        {
            PostService = postService;
            TagService = tagService;
            MembershipService = membershipService;
            BlogSettings = blogSettings;
            WorkContext = workContext;
        }

        protected async Task<IViewComponentResult> Posts(int pageIndex, IEnumerable<BlogPost> model)
        {
            var userIds = model.Select(x => x.UserId).Distinct();
            var userNames = (await MembershipService.Value.GetUsers(WorkContext.CurrentTenant.Id, x => userIds.Contains(x.Id))).ToDictionary(k => k.Id, v => v.UserName);

            int total = await PostService.Value.CountAsync();

            ViewBag.PageCount = (int)Math.Ceiling((double)total / BlogSettings.ItemsPerPage);
            ViewBag.PageIndex = pageIndex;
            ViewBag.UserNames = userNames;

            var tags = await TagService.Value.FindAsync();
            ViewBag.Tags = tags.ToDictionary(k => k.Id, v => v.Name);
            ViewBag.TagUrls = tags.ToDictionary(k => k.Id, v => v.UrlSlug);

            return View("Mantle.Web.ContentManagement.Areas.Admin.Blog.Views.BlogContent.Index", model);
        }
    }
}