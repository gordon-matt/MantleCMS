using System.Collections.Generic;

namespace Mantle.Plugins.Messaging.Forums.Models
{
    public class ActiveDiscussionsModel
    {
        public ActiveDiscussionsModel()
        {
            ForumTopics = new List<ForumTopicRowModel>();
        }

        public IList<ForumTopicRowModel> ForumTopics { get; private set; }

        public bool ViewAllLinkEnabled { get; set; }

        public bool ActiveDiscussionsFeedEnabled { get; set; }

        public int TopicPageSize { get; set; }

        public int TopicTotalRecords { get; set; }

        public int TopicPageIndex { get; set; }

        public int PostsPageSize { get; set; }
    }
}