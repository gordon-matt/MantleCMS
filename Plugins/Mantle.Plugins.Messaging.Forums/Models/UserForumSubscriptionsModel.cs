using System.Collections.Generic;

namespace Mantle.Plugins.Messaging.Forums.Models
{
    public class UserForumSubscriptionsModel
    {
        public UserForumSubscriptionsModel()
        {
            this.ForumSubscriptions = new List<ForumSubscriptionModel>();
        }

        public IList<ForumSubscriptionModel> ForumSubscriptions { get; set; }

        public PagerModel PagerModel { get; set; }

        #region Nested classes

        public class ForumSubscriptionModel
        {
            public int Id { get; set; }

            public int ForumId { get; set; }

            public int ForumTopicId { get; set; }

            public bool TopicSubscription { get; set; }

            public string Title { get; set; }

            public string Slug { get; set; }
        }

        #endregion Nested classes
    }
}