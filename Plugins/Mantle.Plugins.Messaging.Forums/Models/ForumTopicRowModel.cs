using Mantle.Plugins.Messaging.Forums.Data.Domain;

namespace Mantle.Plugins.Messaging.Forums.Models
{
    public class ForumTopicRowModel
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public string SeName { get; set; }

        public int LastPostId { get; set; }

        public int NumPosts { get; set; }

        public int Views { get; set; }

        public int NumReplies { get; set; }

        public ForumTopicType TopicType { get; set; }

        public string UserId { get; set; }

        public bool AllowViewingProfiles { get; set; }

        public string UserName { get; set; }

        //posts
        public int TotalPostPages { get; set; }
    }
}