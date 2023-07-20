using Extenso.Collections.Generic;
using Mantle.Plugins.Messaging.Forums.Data.Domain;
using Mantle.Security.Membership;

namespace Mantle.Plugins.Messaging.Forums.Services
{
    public interface IForumService
    {
        Task DeleteForumGroup(ForumGroup forumGroup);

        Task<ForumGroup> GetForumGroupById(int forumGroupId);

        Task<IEnumerable<ForumGroup>> GetAllForumGroups();

        Task InsertForumGroup(ForumGroup forumGroup);

        Task UpdateForumGroup(ForumGroup forumGroup);

        Task DeleteForum(Forum forum);

        Task<Forum> GetForumById(int forumId);

        Task<IEnumerable<Forum>> GetAllForumsByGroupId(int forumGroupId);

        Task InsertForum(Forum forum);

        Task UpdateForum(Forum forum);

        Task DeleteTopic(ForumTopic forumTopic);

        Task<ForumTopic> GetTopicById(int forumTopicId);

        Task<ForumTopic> GetTopicById(int forumTopicId, bool increaseViews);

        Task<IPagedCollection<ForumTopic>> GetAllTopics(
            int forumId = 0,
            string userId = null,
            string keywords = null,
            ForumSearchType searchType = ForumSearchType.All,
            int limitDays = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue);

        Task<IPagedCollection<ForumTopic>> GetActiveTopics(
            int forumId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue);

        Task InsertTopic(ForumTopic forumTopic, bool sendNotifications);

        Task UpdateTopic(ForumTopic forumTopic);

        Task<ForumTopic> MoveTopic(int forumTopicId, int newForumId);

        Task DeletePost(ForumPost forumPost);

        Task<ForumPost> GetPostById(int forumPostId);

        Task<IPagedCollection<ForumPost>> GetAllPosts(
            int forumTopicId = 0,
            string userId = null,
            string keywords = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue);

        Task<IPagedCollection<ForumPost>> GetAllPosts(
            int forumTopicId = 0,
            string userId = null,
            string keywords = null,
            bool ascSort = false,
            int pageIndex = 0,
            int pageSize = int.MaxValue);

        Task InsertPost(ForumPost forumPost, bool sendNotifications);

        Task UpdatePost(ForumPost forumPost);

        Task DeletePrivateMessage(PrivateMessage privateMessage);

        Task<PrivateMessage> GetPrivateMessageById(int privateMessageId);

        Task<IPagedCollection<PrivateMessage>> GetAllPrivateMessages(
            string fromCustomerId,
            string toCustomerId,
            bool? isRead,
            bool? isDeletedByAuthor,
            bool? isDeletedByRecipient,
            string keywords,
            int pageIndex = 0,
            int pageSize = int.MaxValue);

        Task InsertPrivateMessage(PrivateMessage privateMessage);

        Task UpdatePrivateMessage(PrivateMessage privateMessage);

        Task DeleteSubscription(ForumSubscription forumSubscription);

        Task<ForumSubscription> GetSubscriptionById(int forumSubscriptionId);

        Task<IPagedCollection<ForumSubscription>> GetAllSubscriptions(
            string userId = null,
            int forumId = 0,
            int topicId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue);

        Task InsertSubscription(ForumSubscription forumSubscription);

        Task UpdateSubscription(ForumSubscription forumSubscription);

        Task<bool> IsUserAllowedToCreateTopic(MantleUser user, Forum forum);

        Task<bool> IsUserAllowedToEditTopic(MantleUser user, ForumTopic topic);

        Task<bool> IsUserAllowedToMoveTopic(MantleUser user, ForumTopic topic);

        Task<bool> IsUserAllowedToDeleteTopic(MantleUser user, ForumTopic topic);

        Task<bool> IsUserAllowedToCreatePost(MantleUser user, ForumTopic topic);

        Task<bool> IsUserAllowedToEditPost(MantleUser user, ForumPost post);

        Task<bool> IsUserAllowedToDeletePost(MantleUser user, ForumPost post);

        Task<bool> IsUserAllowedToSetTopicPriority(MantleUser user);

        Task<bool> IsUserAllowedToSubscribe(MantleUser user);

        Task<int> CalculateTopicPageIndex(int forumTopicId, int pageSize, int postId);
    }
}