namespace Mantle.Plugins.Messaging.Forums.Extensions;

public static class ViewComponentHelperExtensions
{
    extension(IViewComponentHelper component)
    {
        public async Task<IHtmlContent> ActiveDiscussionsSmallAsync() =>
            await component.InvokeAsync("ActiveDiscussionsSmall", null);

        public async Task<IHtmlContent> ForumBreadcrumbsAsync(
            int? forumGroupId = null, int? forumId = null, int? forumTopicId = null) => await component.InvokeAsync("ForumBreadcrumbs", new
            {
                forumGroupId,
                forumId,
                forumTopicId
            });

        public async Task<IHtmlContent> LastPostAsync(int forumPostId, bool showTopic) => await component.InvokeAsync("LastPost", new
        {
            forumPostId,
            showTopic
        });
    }
}