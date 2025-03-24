namespace Mantle.Web.ContentManagement.Areas.Admin.Blog;

public static class ViewComponentHelperExtensions
{
    public static async Task<IHtmlContent> BlogPostsAsync(this IViewComponentHelper component) => await component.InvokeAsync("BlogPosts", null);

    public static async Task<IHtmlContent> BlogPostsByCategoryAsync(this IViewComponentHelper component, string categorySlug) => await component.InvokeAsync("BlogPostsByCategory", new
    {
        categorySlug
    });

    public static async Task<IHtmlContent> BlogPostsByTagAsync(this IViewComponentHelper component, string tagSlug) => await component.InvokeAsync("BlogPostsByTag", new
    {
        tagSlug
    });
}