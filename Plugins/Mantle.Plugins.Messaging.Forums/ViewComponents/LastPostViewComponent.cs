using System;
using System.Threading.Tasks;
using Mantle.Helpers;
using Mantle.Plugins.Messaging.Forums.Extensions;
using Mantle.Plugins.Messaging.Forums.Models;
using Mantle.Plugins.Messaging.Forums.Services;
using Mantle.Security.Membership;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Plugins.Messaging.Forums.ViewComponents
{
    [ViewComponent(Name = "LastPost")]
    public class LastPostViewComponent : ViewComponent
    {
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IForumService forumService;
        private readonly IMembershipService membershipService;
        private readonly ForumSettings forumSettings;

        public LastPostViewComponent(
            IDateTimeHelper dateTimeHelper,
            IForumService forumService,
            IMembershipService membershipService,
            ForumSettings forumSettings)
        {
            this.dateTimeHelper = dateTimeHelper;
            this.forumService = forumService;
            this.membershipService = membershipService;
            this.forumSettings = forumSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync(int forumPostId, bool showTopic)
        {
            var post = await forumService.GetPostById(forumPostId);
            var model = new LastPostModel();

            if (post != null)
            {
                var postUser = await membershipService.GetUserById(post.UserId);
                var topic = post.ForumTopic ?? await forumService.GetTopicById(post.TopicId);

                model.Id = post.Id;
                model.ForumTopicId = post.TopicId;
                model.ForumTopicSeName = topic.GetSeName();
                model.ForumTopicSubject = topic.StripTopicSubject();
                model.UserId = post.UserId;
                //model.AllowViewingProfiles = _customerSettings.AllowViewingProfiles; //TODO
                model.AllowViewingProfiles = true;
                model.UserName = await membershipService.GetUserDisplayName(postUser);
                //created on string
                if (forumSettings.RelativeDateTimeFormattingEnabled)
                {
                    model.PostCreatedOnStr = post.CreatedOnUtc.RelativeFormat(true, "f");
                }
                else
                {
                    model.PostCreatedOnStr = dateTimeHelper.ConvertToUserTime(post.CreatedOnUtc, DateTimeKind.Utc).ToString("f");
                }
            }
            model.ShowTopic = showTopic;
            return View(model);
        }
    }
}