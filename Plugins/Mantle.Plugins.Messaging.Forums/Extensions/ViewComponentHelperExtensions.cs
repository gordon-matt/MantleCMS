namespace Mantle.Plugins.Messaging.Forums.Extensions;

public static class ViewComponentHelperExtensions
{
    public static async Task<IHtmlContent> ActiveDiscussionsSmallAsync(this IViewComponentHelper component) => await component.InvokeAsync("ActiveDiscussionsSmall", null);

    public static async Task<IHtmlContent> ForumBreadcrumbsAsync(this IViewComponentHelper component, int? forumGroupId = null, int? forumId = null, int? forumTopicId = null) => await component.InvokeAsync("ForumBreadcrumbs", new
    {
        forumGroupId,
        forumId,
        forumTopicId
    });

    public static async Task<IHtmlContent> LastPostAsync(this IViewComponentHelper component, int forumPostId, bool showTopic) => await component.InvokeAsync("LastPost", new
    {
        forumPostId,
        showTopic
    });
}