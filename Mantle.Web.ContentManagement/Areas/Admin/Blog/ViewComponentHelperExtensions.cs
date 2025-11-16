namespace Mantle.Web.ContentManagement.Areas.Admin.Blog;

public static class ViewComponentHelperExtensions
{
    extension(IViewComponentHelper component)
    {
        public async Task<IHtmlContent> BlogPostsAsync() =>
            await component.InvokeAsync("BlogPosts", null);

        public async Task<IHtmlContent> BlogPostsByCategoryAsync(string categorySlug) =>
            await component.InvokeAsync("BlogPostsByCategory", new { categorySlug });

        public async Task<IHtmlContent> BlogPostsByTagAsync(string tagSlug) =>
            await component.InvokeAsync("BlogPostsByTag", new { tagSlug });
    }
}