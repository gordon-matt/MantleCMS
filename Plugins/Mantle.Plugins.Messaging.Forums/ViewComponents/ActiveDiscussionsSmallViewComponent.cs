using Mantle.Plugins.Messaging.Forums.Extensions;
using Mantle.Plugins.Messaging.Forums.Models;
using Mantle.Plugins.Messaging.Forums.Services;

namespace Mantle.Plugins.Messaging.Forums.ViewComponents;

[ViewComponent(Name = "ActiveDiscussionsSmall")]
public class ActiveDiscussionsSmallViewComponent : ViewComponent
{
    private readonly IForumService forumService;
    private readonly IMembershipService membershipService;
    private readonly ForumSettings forumSettings;

    public ActiveDiscussionsSmallViewComponent(
        IForumService forumService,
        IMembershipService membershipService,
        ForumSettings forumSettings)
    {
        this.forumService = forumService;
        this.membershipService = membershipService;
        this.forumSettings = forumSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!forumSettings.ForumsEnabled)
        {
            return Content("");
        }

        var topics = await forumService.GetActiveTopics(0, 0, forumSettings.HomePageActiveDiscussionsTopicCount);
        if (topics.Count == 0)
        {
            return Content(string.Empty);
        }

        var model = new ActiveDiscussionsModel
        {
            ViewAllLinkEnabled = true,
            ActiveDiscussionsFeedEnabled = forumSettings.ActiveDiscussionsFeedEnabled,
            PostsPageSize = forumSettings.PostsPageSize
        };

        foreach (var topic in topics)
        {
            var topicModel = await PrepareForumTopicRowModel(topic);
            model.ForumTopics.Add(topicModel);
        }

        return View(model);
    }

    private async Task<ForumTopicRowModel> PrepareForumTopicRowModel(ForumTopic topic)
    {
        var user = await membershipService.GetUserById(topic.UserId);

        var topicModel = new ForumTopicRowModel
        {
            Id = topic.Id,
            Subject = topic.Subject,
            SeName = topic.GetSeName(),
            LastPostId = topic.LastPostId,
            NumPosts = topic.NumPosts,
            Views = topic.Views,
            NumReplies = topic.NumReplies,
            TopicType = topic.TopicType,
            UserId = topic.UserId,
            //AllowViewingProfiles = _customerSettings.AllowViewingProfiles, //TODO
            AllowViewingProfiles = true,
            UserName = await membershipService.GetUserDisplayName(user)
        };

        var posts = await forumService.GetAllPosts(topic.Id, null, string.Empty, 1, forumSettings.PostsPageSize);
        topicModel.TotalPostPages = posts.ItemCount;

        return topicModel;
    }
}