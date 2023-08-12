using Mantle.Web.ContentManagement.Areas.Admin.Blog.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Pages;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.ViewComponents;

[ViewComponent(Name = "BlogPosts")]
public class BlogPostsViewComponent : BaseBlogPostsViewComponent
{
    public BlogPostsViewComponent(
        Lazy<IBlogPostService> postService,
        Lazy<IBlogTagService> tagService,
        Lazy<IMembershipService> membershipService,
        BlogSettings blogSettings,
        IWorkContext workContext)
        : base(postService, tagService, membershipService, blogSettings, workContext)
    {
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!await PageSecurityHelper.CheckUserHasAccessToBlog(User))
        {
            return Content("");
        }

        int tenantId = WorkContext.CurrentTenant.Id;

        string pageIndexParam = Request.Query["pageIndex"];
        int pageIndex = string.IsNullOrEmpty(pageIndexParam)
            ? 1
            : Convert.ToInt32(pageIndexParam);

        List<BlogPost> model = null;
        using (var connection = PostService.Value.OpenConnection())
        {
            model = await connection.Query(x => x.TenantId == tenantId)
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .OrderByDescending(x => x.DateCreatedUtc)
                .Skip((pageIndex - 1) * BlogSettings.ItemsPerPage)
                .Take(BlogSettings.ItemsPerPage)
                .ToListAsync();
        }

        return await Posts(pageIndex, model);
    }
}