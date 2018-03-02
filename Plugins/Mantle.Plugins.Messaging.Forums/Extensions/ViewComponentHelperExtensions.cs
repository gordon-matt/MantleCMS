using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Plugins.Messaging.Forums.Extensions
{
    public static class ViewComponentHelperExtensions
    {
        public static async Task<IHtmlContent> ActiveDiscussionsSmallAsync(this IViewComponentHelper component)
        {
            return await component.InvokeAsync("ActiveDiscussionsSmall", null);
        }

        public static async Task<IHtmlContent> ForumBreadcrumbsAsync(this IViewComponentHelper component, int? forumGroupId = null, int? forumId = null, int? forumTopicId = null)
        {
            return await component.InvokeAsync("ForumBreadcrumbs", new
            {
                forumGroupId = forumGroupId,
                forumId = forumId,
                forumTopicId = forumTopicId
            });
        }

        public static async Task<IHtmlContent> LastPostAsync(this IViewComponentHelper component, int forumPostId, bool showTopic)
        {
            return await component.InvokeAsync("LastPost", new
            {
                forumPostId = forumPostId,
                showTopic = showTopic
            });
        }
    }
}