﻿using Mantle.Web.ContentManagement.Areas.Admin.Blog.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.ViewComponents;

[ViewComponent(Name = "BlogPostsByTag")]
public class BlogPostsByTagViewComponent : BaseBlogPostsViewComponent
{
    private readonly IStringLocalizer T;

    public BlogPostsByTagViewComponent(
        Lazy<IBlogPostService> postService,
        Lazy<IBlogTagService> tagService,
        Lazy<IMembershipService> membershipService,
        BlogSettings blogSettings,
        IWorkContext workContext,
        IStringLocalizer localizer)
        : base(postService, tagService, membershipService, blogSettings, workContext)
    {
        T = localizer;
    }

    public async Task<IViewComponentResult> InvokeAsync(string tagSlug)
    {
        int tenantId = WorkContext.CurrentTenant.Id;

        var tag = await TagService.Value.FindOneAsync(new SearchOptions<BlogTag>
        {
            Query = x =>
                x.TenantId == tenantId
                && x.UrlSlug == tagSlug
        });

        if (tag == null)
        {
            throw new EntityNotFoundException(string.Concat(
                "Could not find a blog tag with slug, '", tagSlug, "'"));
        }

        WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Blog.Title].Value, Url.Action("Index"));
        WorkContext.Breadcrumbs.Add(tag.Name);

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
                .Where(x => x.Tags.Any(y => y.TagId == tag.Id))
                .OrderByDescending(x => x.DateCreatedUtc)
                .Skip((pageIndex - 1) * BlogSettings.ItemsPerPage)
                .Take(BlogSettings.ItemsPerPage)
                .ToListAsync();
        }

        return await Posts(pageIndex, model);
    }
}