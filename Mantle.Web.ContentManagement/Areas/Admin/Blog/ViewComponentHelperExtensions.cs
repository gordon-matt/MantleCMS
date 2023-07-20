using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog
{
    public static class ViewComponentHelperExtensions
    {
        public static async Task<IHtmlContent> BlogPostsAsync(this IViewComponentHelper component)
        {
            return await component.InvokeAsync("BlogPosts", null);
        }

        public static async Task<IHtmlContent> BlogPostsByCategoryAsync(this IViewComponentHelper component, string categorySlug)
        {
            return await component.InvokeAsync("BlogPostsByCategory", new
            {
                categorySlug = categorySlug
            });
        }

        public static async Task<IHtmlContent> BlogPostsByTagAsync(this IViewComponentHelper component, string tagSlug)
        {
            return await component.InvokeAsync("BlogPostsByTag", new
            {
                tagSlug = tagSlug
            });
        }
    }
}