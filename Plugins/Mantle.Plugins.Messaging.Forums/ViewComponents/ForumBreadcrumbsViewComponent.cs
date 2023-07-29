using Mantle.Plugins.Messaging.Forums.Data.Domain;
using Mantle.Plugins.Messaging.Forums.Extensions;
using Mantle.Plugins.Messaging.Forums.Models;
using Mantle.Plugins.Messaging.Forums.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Plugins.Messaging.Forums.ViewComponents
{
    [ViewComponent(Name = "ForumBreadcrumbs")]
    public class ForumBreadcrumbsViewComponent : ViewComponent
    {
        private readonly IForumService forumService;

        public ForumBreadcrumbsViewComponent(IForumService forumService)
        {
            this.forumService = forumService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? forumGroupId, int? forumId, int? forumTopicId)
        {
            var model = new ForumBreadcrumbModel();

            ForumTopic topic = null;
            if (forumTopicId.HasValue)
            {
                topic = await forumService.GetTopicById(forumTopicId.Value);
                if (topic != null)
                {
                    model.ForumTopicId = topic.Id;
                    model.ForumTopicSubject = topic.Subject;
                    model.ForumTopicSeName = topic.GetSeName();
                }
            }

            var forum = await forumService.GetForumById(topic != null ? topic.ForumId : (forumId ?? 0));
            if (forum != null)
            {
                model.ForumId = forum.Id;
                model.ForumName = forum.Name;
                model.ForumSeName = forum.GetSeName();
            }

            var forumGroup = await forumService.GetForumGroupById(forum != null ? forum.ForumGroupId : (forumGroupId ?? 0));
            if (forumGroup != null)
            {
                model.ForumGroupId = forumGroup.Id;
                model.ForumGroupName = forumGroup.Name;
                model.ForumGroupSeName = forumGroup.GetSeName();
            }

            return View(model);
        }
    }
}