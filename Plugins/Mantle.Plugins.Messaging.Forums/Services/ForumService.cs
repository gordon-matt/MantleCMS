using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extenso.Collections.Generic;
using Extenso.Data.Entity;
using Mantle.Caching;
using Mantle.Data;
using Mantle.Exceptions;
using Mantle.Plugins.Messaging.Forums.Data.Domain;
using Mantle.Security.Membership;
using Mantle.Web;
using Mantle.Web.Configuration.Services;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Plugins.Messaging.Forums.Services
{
    public class ForumService : IForumService
    {
        #region Private Members

        private readonly ForumSettings forumSettings;
        private readonly ICacheManager cacheManager;
        private readonly IGenericAttributeService genericAttributeService;
        private readonly IMembershipService membershipService;
        private readonly IRepository<Forum> forumRepository;
        private readonly IRepository<ForumGroup> forumGroupRepository;
        private readonly IRepository<ForumPost> forumPostRepository;
        private readonly IRepository<ForumSubscription> forumSubscriptionRepository;
        private readonly IRepository<ForumTopic> forumTopicRepository;
        private readonly IRepository<PrivateMessage> forumPrivateMessageRepository;
        private readonly IWorkContext workContext;

        private const string CacheKey_ForumGroupAll = "Mantle.ForumGroup.Tenant_{0}.All";
        private const string CacheKey_ForumAllByForumGroupId = "Mantle.Forum.Tenant_{0}.AllByForumGroupId-{1}";
        private const string CacheKey_Pattern_ForumGroup = "Mantle.ForumGroup.Tenant_{0}.*";
        private const string CacheKey_Pattern_Forum = "Mantle.Forum.Tenant_{0}.*";

        #endregion Private Members

        #region Ctor

        public ForumService(
            ForumSettings forumSettings,
            ICacheManager cacheManager,
            IGenericAttributeService genericAttributeService,
            IMembershipService membershipService,
            IRepository<Forum> forumRepository,
            IRepository<ForumGroup> forumGroupRepository,
            IRepository<ForumPost> forumPostRepository,
            IRepository<ForumSubscription> forumSubscriptionRepository,
            IRepository<ForumTopic> forumTopicRepository,
            IRepository<PrivateMessage> forumPrivateMessageRepository,
            IWorkContext workContext)
        {
            this.cacheManager = cacheManager;
            this.forumGroupRepository = forumGroupRepository;
            this.forumPostRepository = forumPostRepository;
            this.forumPrivateMessageRepository = forumPrivateMessageRepository;
            this.forumRepository = forumRepository;
            this.forumSettings = forumSettings;
            this.forumSubscriptionRepository = forumSubscriptionRepository;
            this.forumTopicRepository = forumTopicRepository;
            this.genericAttributeService = genericAttributeService;
            this.membershipService = membershipService;
            this.workContext = workContext;
        }

        #endregion Ctor

        #region Methods

        public virtual async Task DeleteForumGroup(ForumGroup forumGroup)
        {
            if (forumGroup == null)
            {
                throw new ArgumentNullException("forumGroup");
            }

            await forumGroupRepository.DeleteAsync(forumGroup);

            int tenantId = workContext.CurrentTenant.Id;
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_ForumGroup, tenantId));
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_Forum, tenantId));

            //event notification
            //_eventPublisher.EntityDeleted(forumGroup);
        }

        public virtual async Task<ForumGroup> GetForumGroupById(int forumGroupId)
        {
            if (forumGroupId == 0)
            {
                return null;
            }

            return await forumGroupRepository.FindOneAsync(forumGroupId);
        }

        public virtual async Task<IEnumerable<ForumGroup>> GetAllForumGroups()
        {
            int tenantId = workContext.CurrentTenant.Id;
            string key = string.Format(CacheKey_ForumGroupAll, tenantId);

            return await cacheManager.Get(key, async () =>
            {
                using (var connection = forumGroupRepository.OpenConnection())
                {
                    return await connection
                        .Query(x => x.TenantId == tenantId)
                        .OrderBy(x => x.DisplayOrder)
                        .ToListAsync();
                }
            });
        }

        public virtual async Task InsertForumGroup(ForumGroup forumGroup)
        {
            if (forumGroup == null)
            {
                throw new ArgumentNullException("forumGroup");
            }

            await forumGroupRepository.InsertAsync(forumGroup);

            //cache
            int tenantId = workContext.CurrentTenant.Id;
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_ForumGroup, tenantId));
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_Forum, tenantId));

            //event notification
            //_eventPublisher.EntityInserted(forumGroup);
        }

        public virtual async Task UpdateForumGroup(ForumGroup forumGroup)
        {
            if (forumGroup == null)
            {
                throw new ArgumentNullException("forumGroup");
            }

            await forumGroupRepository.UpdateAsync(forumGroup);

            //cache
            int tenantId = workContext.CurrentTenant.Id;
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_ForumGroup, tenantId));
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_Forum, tenantId));

            //event notification
            //_eventPublisher.EntityUpdated(forumGroup);
        }

        public virtual async Task DeleteForum(Forum forum)
        {
            if (forum == null)
            {
                throw new ArgumentNullException("forum");
            }

            using (var forumTopicConnection = forumTopicRepository.OpenConnection())
            using (var forumSubscriptionConnection = forumSubscriptionRepository.UseConnection(forumTopicConnection))
            {
                //delete forum subscriptions (topics)
                var queryTopicIds = forumTopicConnection.Query(x => x.ForumId == forum.Id).Select(x => x.Id);
                var queryFs1 = forumSubscriptionConnection.Query(x => queryTopicIds.Contains(x.TopicId));

                foreach (var fs in await queryFs1.ToListAsync())
                {
                    await forumSubscriptionRepository.DeleteAsync(fs);
                    //event notification
                    //_eventPublisher.EntityDeleted(fs);
                }

                //delete forum subscriptions (forum)
                var queryFs2 = forumSubscriptionConnection.Query(x => x.ForumId == forum.Id);

                foreach (var fs2 in await queryFs2.ToListAsync())
                {
                    await forumSubscriptionRepository.DeleteAsync(fs2);
                    //event notification
                    //_eventPublisher.EntityDeleted(fs2);
                }
            }

            //delete forum
            await forumRepository.DeleteAsync(forum);

            int tenantId = workContext.CurrentTenant.Id;
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_ForumGroup, tenantId));
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_Forum, tenantId));

            //event notification
            //_eventPublisher.EntityDeleted(forum);
        }

        public virtual async Task<Forum> GetForumById(int forumId)
        {
            if (forumId == 0)
            {
                return null;
            }

            return await forumRepository.FindOneAsync(forumId);
        }

        public virtual async Task<IEnumerable<Forum>> GetAllForumsByGroupId(int forumGroupId)
        {
            int tenantId = workContext.CurrentTenant.Id;
            string key = string.Format(CacheKey_ForumAllByForumGroupId, tenantId, forumGroupId);

            return await cacheManager.Get(key, async () =>
            {
                using (var connection = forumRepository.OpenConnection())
                {
                    return await connection
                        .Query(f => f.ForumGroupId == forumGroupId)
                        .OrderBy(f => f.DisplayOrder)
                        .ToListAsync();
                }
            });
        }

        public virtual async Task InsertForum(Forum forum)
        {
            if (forum == null)
            {
                throw new ArgumentNullException("forum");
            }

            await forumRepository.InsertAsync(forum);

            int tenantId = workContext.CurrentTenant.Id;
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_ForumGroup, tenantId));
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_Forum, tenantId));

            //event notification
            //_eventPublisher.EntityInserted(forum);
        }

        public virtual async Task UpdateForum(Forum forum)
        {
            if (forum == null)
            {
                throw new ArgumentNullException("forum");
            }

            await forumRepository.UpdateAsync(forum);

            int tenantId = workContext.CurrentTenant.Id;
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_ForumGroup, tenantId));
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_Forum, tenantId));

            //event notification
            //_eventPublisher.EntityUpdated(forum);
        }

        public virtual async Task DeleteTopic(ForumTopic forumTopic)
        {
            if (forumTopic == null)
            {
                throw new ArgumentNullException("forumTopic");
            }

            string userId = forumTopic.UserId;
            int forumId = forumTopic.ForumId;

            //delete topic
            await forumTopicRepository.DeleteAsync(forumTopic);

            using (var forumSubscriptionConnection = forumSubscriptionRepository.OpenConnection())
            {
                //delete forum subscriptions
                var forumSubscriptions = forumSubscriptionConnection
                    .Query(x => x.TopicId == forumTopic.Id)
                    .ToList();

                foreach (var fs in forumSubscriptions)
                {
                    await forumSubscriptionRepository.DeleteAsync(fs);
                    //event notification
                    //_eventPublisher.EntityDeleted(fs);
                }
            }

            //update stats
            await UpdateForumStats(forumId);
            await UpdateUserStats(userId);

            int tenantId = workContext.CurrentTenant.Id;
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_ForumGroup, tenantId));
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_Forum, tenantId));

            //event notification
            //_eventPublisher.EntityDeleted(forumTopic);
        }

        public virtual async Task<ForumTopic> GetTopicById(int forumTopicId)
        {
            return await GetTopicById(forumTopicId, false);
        }

        public virtual async Task<ForumTopic> GetTopicById(int forumTopicId, bool increaseViews)
        {
            if (forumTopicId == 0)
            {
                return null;
            }

            var forumTopic = await forumTopicRepository.FindOneAsync(forumTopicId);
            if (forumTopic == null)
            {
                return null;
            }

            if (increaseViews)
            {
                forumTopic.Views = ++forumTopic.Views;
                await UpdateTopic(forumTopic);
            }

            return forumTopic;
        }

        public virtual async Task<IPagedList<ForumTopic>> GetAllTopics(
            int forumId = 0,
            string userId = null,
            string keywords = null,
            ForumSearchType searchType = ForumSearchType.All,
            int limitDays = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            DateTime? limitDate = null;
            if (limitDays > 0)
            {
                limitDate = DateTime.UtcNow.AddDays(-limitDays);
            }
            //we need to cast it to int, otherwise it won't work in SQLCE4
            //we cannot use string.IsNullOrEmpty in query because it causes SqlCeException on SQLCE4
            bool searchKeywords = !string.IsNullOrEmpty(keywords);
            bool searchTopicTitles = searchType == ForumSearchType.All || searchType == ForumSearchType.TopicTitlesOnly;
            bool searchPostText = searchType == ForumSearchType.All || searchType == ForumSearchType.PostTextOnly;

            using (var forumTopicConnection = forumTopicRepository.OpenConnection())
            using (var forumPostConnection = forumPostRepository.UseConnection(forumTopicConnection))
            {
                var query1 = from ft in forumTopicConnection.Query()
                             join fp in forumPostConnection.Query() on ft.Id equals fp.TopicId
                             where
                             (forumId == 0 || ft.ForumId == forumId) &&
                             (userId == null || ft.UserId == userId) &&
                             (
                                !searchKeywords ||
                                (searchTopicTitles && ft.Subject.Contains(keywords)) ||
                                (searchPostText && fp.Text.Contains(keywords))) &&
                             (!limitDate.HasValue || limitDate.Value <= ft.LastPostTime)
                             select ft.Id;

                var query2 = from ft in forumTopicConnection.Query()
                             where query1.Contains(ft.Id)
                             orderby ft.TopicType descending, ft.LastPostTime descending, ft.Id descending
                             select ft;

                return await Task.FromResult(new PagedList<ForumTopic>(query2, pageIndex, pageSize));
            }
        }

        public virtual async Task<IPagedList<ForumTopic>> GetActiveTopics(
            int forumId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            using (var forumTopicConnection = forumTopicRepository.OpenConnection())
            {
                var query1 = from ft in forumTopicConnection.Query()
                             where
                             (forumId == 0 || ft.ForumId == forumId) &&
                             (ft.LastPostTime.HasValue)
                             select ft.Id;

                var query2 = from ft in forumTopicConnection.Query()
                             where query1.Contains(ft.Id)
                             orderby ft.LastPostTime descending
                             select ft;

                var topics = new PagedList<ForumTopic>(query2, pageIndex, pageSize);

                return await Task.FromResult(topics);
            }
        }

        public virtual async Task InsertTopic(ForumTopic forumTopic, bool sendNotifications)
        {
            if (forumTopic == null)
            {
                throw new ArgumentNullException("forumTopic");
            }

            await forumTopicRepository.InsertAsync(forumTopic);

            //update stats
            await UpdateForumStats(forumTopic.ForumId);

            //cache
            int tenantId = workContext.CurrentTenant.Id;
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_ForumGroup, tenantId));
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_Forum, tenantId));

            //event notification
            //_eventPublisher.EntityInserted(forumTopic);

            //send notifications
            if (sendNotifications)
            {
                var forum = forumTopic.Forum ?? await forumRepository.FindOneAsync(forumTopic.ForumId);
                var subscriptions = await GetAllSubscriptions(forumId: forum.Id);
                //var languageId = workContext.WorkingLanguage.Id;

                foreach (var subscription in subscriptions)
                {
                    if (subscription.UserId == forumTopic.UserId)
                    {
                        continue;
                    }

                    //if (!string.IsNullOrEmpty(subscription.User.Email))
                    //{
                    //    //_workflowMessageService.SendNewForumTopicMessage(subscription.User, forumTopic,
                    //    //    forum, languageId);
                    //}
                }
            }
        }

        public virtual async Task UpdateTopic(ForumTopic forumTopic)
        {
            if (forumTopic == null)
            {
                throw new ArgumentNullException("forumTopic");
            }

            await forumTopicRepository.UpdateAsync(forumTopic);

            int tenantId = workContext.CurrentTenant.Id;
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_ForumGroup, tenantId));
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_Forum, tenantId));

            //event notification
            //_eventPublisher.EntityUpdated(forumTopic);
        }

        public virtual async Task<ForumTopic> MoveTopic(int forumTopicId, int newForumId)
        {
            var forumTopic = await GetTopicById(forumTopicId);

            if (forumTopic == null)
            {
                return null;
            }

            if (await IsUserAllowedToMoveTopic(workContext.CurrentUser, forumTopic))
            {
                int previousForumId = forumTopic.ForumId;
                var newForum = await GetForumById(newForumId);

                if (newForum != null)
                {
                    if (previousForumId != newForumId)
                    {
                        forumTopic.ForumId = newForum.Id;
                        forumTopic.UpdatedOnUtc = DateTime.UtcNow;
                        await UpdateTopic(forumTopic);

                        //update forum stats
                        await UpdateForumStats(previousForumId);
                        await UpdateForumStats(newForumId);
                    }
                }
            }
            return forumTopic;
        }

        public virtual async Task DeletePost(ForumPost forumPost)
        {
            if (forumPost == null)
            {
                throw new ArgumentNullException("forumPost");
            }

            int forumTopicId = forumPost.TopicId;
            string userId = forumPost.UserId;
            var forumTopic = await GetTopicById(forumTopicId);
            int forumId = forumTopic.ForumId;

            //delete topic if it was the first post
            bool deleteTopic = false;
            ForumPost firstPost = forumTopic.GetFirstPost(this);
            if (firstPost != null && firstPost.Id == forumPost.Id)
            {
                deleteTopic = true;
            }

            //delete forum post
            await forumPostRepository.DeleteAsync(forumPost);

            //delete topic
            if (deleteTopic)
            {
                await DeleteTopic(forumTopic);
            }

            //update stats
            if (!deleteTopic)
            {
                await UpdateForumTopicStats(forumTopicId);
            }
            await UpdateForumStats(forumId);
            await UpdateUserStats(userId);

            //clear cache
            int tenantId = workContext.CurrentTenant.Id;
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_ForumGroup, tenantId));
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_Forum, tenantId));

            //event notification
            //_eventPublisher.EntityDeleted(forumPost);
        }

        public virtual async Task<ForumPost> GetPostById(int forumPostId)
        {
            if (forumPostId == 0)
            {
                return null;
            }

            return await forumPostRepository.FindOneAsync(forumPostId);
        }

        public virtual async Task<IPagedList<ForumPost>> GetAllPosts(
            int forumTopicId = 0,
            string userId = null,
            string keywords = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            return await GetAllPosts(forumTopicId, userId, keywords, true, pageIndex, pageSize);
        }

        public virtual async Task<IPagedList<ForumPost>> GetAllPosts(
            int forumTopicId = 0,
            string userId = null,
            string keywords = null,
            bool ascSort = false,
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            using (var forumPostConnection = forumPostRepository.OpenConnection())
            {
                var query = forumPostConnection.Query();
                if (forumTopicId > 0)
                {
                    query = query.Where(x => forumTopicId == x.TopicId);
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    query = query.Where(x => userId == x.UserId);
                }
                if (!string.IsNullOrEmpty(keywords))
                {
                    query = query.Where(x => x.Text.Contains(keywords));
                }

                query = ascSort ?
                    query.OrderBy(x => x.CreatedOnUtc).ThenBy(fp => fp.Id) :
                    query.OrderByDescending(x => x.CreatedOnUtc).ThenBy(fp => fp.Id);

                return await Task.FromResult(new PagedList<ForumPost>(query, pageIndex, pageSize));
            }
        }

        public virtual async Task InsertPost(ForumPost forumPost, bool sendNotifications)
        {
            if (forumPost == null)
            {
                throw new ArgumentNullException("forumPost");
            }

            await forumPostRepository.InsertAsync(forumPost);

            //update stats
            string userId = forumPost.UserId;
            var forumTopic = await GetTopicById(forumPost.TopicId);
            int forumId = forumTopic.ForumId;
            await UpdateForumTopicStats(forumPost.TopicId);
            await UpdateForumStats(forumId);
            await UpdateUserStats(userId);

            //clear cache
            int tenantId = workContext.CurrentTenant.Id;
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_ForumGroup, tenantId));
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_Forum, tenantId));

            //event notification
            //_eventPublisher.EntityInserted(forumPost);

            ////notifications
            //if (sendNotifications)
            //{
            //    var forum = forumTopic.Forum ?? await forumRepository.FindOneAsync(forumTopic.ForumId);
            //    var subscriptions = await GetAllSubscriptions(topicId: forumTopic.Id);

            //    //var languageId = workContext.WorkingLanguage.Id;

            //    int friendlyTopicPageIndex = (await CalculateTopicPageIndex(
            //        forumPost.TopicId,
            //        forumSettings.PostsPageSize > 0 ? forumSettings.PostsPageSize : 10,
            //        forumPost.Id)) + 1;

            //    foreach (ForumSubscription subscription in subscriptions)
            //    {
            //        if (subscription.UserId == forumPost.UserId)
            //        {
            //            continue;
            //        }

            //        //if (!string.IsNullOrEmpty(subscription.User.Email))
            //        //{
            //        //    //_workflowMessageService.SendNewForumPostMessage(subscription.User, forumPost,
            //        //    //    forumTopic, forum, friendlyTopicPageIndex, languageId);
            //        //}
            //    }
            //}
        }

        public virtual async Task UpdatePost(ForumPost forumPost)
        {
            //validation
            if (forumPost == null)
            {
                throw new ArgumentNullException("forumPost");
            }

            await forumPostRepository.UpdateAsync(forumPost);

            int tenantId = workContext.CurrentTenant.Id;
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_ForumGroup, tenantId));
            cacheManager.RemoveByPattern(string.Format(CacheKey_Pattern_Forum, tenantId));

            //event notification
            //_eventPublisher.EntityUpdated(forumPost);
        }

        public virtual async Task DeletePrivateMessage(PrivateMessage privateMessage)
        {
            if (privateMessage == null)
            {
                throw new ArgumentNullException("privateMessage");
            }

            await forumPrivateMessageRepository.DeleteAsync(privateMessage);

            //event notification
            //_eventPublisher.EntityDeleted(privateMessage);
        }

        public virtual async Task<PrivateMessage> GetPrivateMessageById(int privateMessageId)
        {
            if (privateMessageId == 0)
            {
                return null;
            }

            return await forumPrivateMessageRepository.FindOneAsync(privateMessageId);
        }

        public virtual async Task<IPagedList<PrivateMessage>> GetAllPrivateMessages(
            string fromUserId,
            string toUserId,
            bool? isRead,
            bool? isDeletedByAuthor,
            bool? isDeletedByRecipient,
            string keywords,
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            using (var forumPrivateMessageConnection = forumPrivateMessageRepository.OpenConnection())
            {
                var query = forumPrivateMessageConnection.Query();
                //if (storeId > 0)
                //    query = query.Where(pm => storeId == pm.StoreId);
                if (!string.IsNullOrEmpty(fromUserId))
                {
                    query = query.Where(pm => fromUserId == pm.FromUserId);
                }
                if (!string.IsNullOrEmpty(toUserId))
                {
                    query = query.Where(pm => toUserId == pm.ToUserId);
                }
                if (isRead.HasValue)
                {
                    query = query.Where(pm => isRead.Value == pm.IsRead);
                }
                if (isDeletedByAuthor.HasValue)
                {
                    query = query.Where(pm => isDeletedByAuthor.Value == pm.IsDeletedByAuthor);
                }
                if (isDeletedByRecipient.HasValue)
                {
                    query = query.Where(pm => isDeletedByRecipient.Value == pm.IsDeletedByRecipient);
                }
                if (!string.IsNullOrEmpty(keywords))
                {
                    query = query.Where(pm => pm.Subject.Contains(keywords));
                    query = query.Where(pm => pm.Text.Contains(keywords));
                }
                query = query.OrderByDescending(pm => pm.CreatedOnUtc);

                return await Task.FromResult(new PagedList<PrivateMessage>(query, pageIndex, pageSize));
            }
        }

        public virtual async Task InsertPrivateMessage(PrivateMessage privateMessage)
        {
            if (privateMessage == null)
            {
                throw new ArgumentNullException("privateMessage");
            }

            await forumPrivateMessageRepository.InsertAsync(privateMessage);

            //event notification
            //_eventPublisher.EntityInserted(privateMessage);

            var userTo = await membershipService.GetUserById(privateMessage.ToUserId);
            if (userTo == null)
            {
                throw new MantleException("Recipient could not be loaded");
            }

            //UI notification
            genericAttributeService.SaveAttribute(userTo, SystemUserAttributeNames.NotifiedAboutNewPrivateMessages, false);

            //Email notification
            if (forumSettings.NotifyAboutPrivateMessages)
            {
                //_workflowMessageService.SendPrivateMessageNotification(privateMessage, workContext.WorkingLanguage.Id);
            }
        }

        public virtual async Task UpdatePrivateMessage(PrivateMessage privateMessage)
        {
            if (privateMessage == null)
            {
                throw new ArgumentNullException("privateMessage");
            }

            if (privateMessage.IsDeletedByAuthor && privateMessage.IsDeletedByRecipient)
            {
                await forumPrivateMessageRepository.DeleteAsync(privateMessage);
                //event notification
                //_eventPublisher.EntityDeleted(privateMessage);
            }
            else
            {
                await forumPrivateMessageRepository.UpdateAsync(privateMessage);
                //event notification
                //_eventPublisher.EntityUpdated(privateMessage);
            }
        }

        public virtual async Task DeleteSubscription(ForumSubscription forumSubscription)
        {
            if (forumSubscription == null)
            {
                throw new ArgumentNullException("forumSubscription");
            }

            await forumSubscriptionRepository.DeleteAsync(forumSubscription);

            //event notification
            //_eventPublisher.EntityDeleted(forumSubscription);
        }

        public virtual async Task<ForumSubscription> GetSubscriptionById(int forumSubscriptionId)
        {
            if (forumSubscriptionId == 0)
            {
                return null;
            }

            return await forumSubscriptionRepository.FindOneAsync(forumSubscriptionId);
        }

        public virtual async Task<IPagedList<ForumSubscription>> GetAllSubscriptions(
            string userId = null,
            int forumId = 0,
            int topicId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            bool isLockedOut = string.IsNullOrEmpty(userId)
                ? false
                : (await membershipService.GetUserById(userId)).IsLockedOut;

            using (var forumSubscriptionConnection = forumSubscriptionRepository.OpenConnection())
            {
                var fsQuery = forumSubscriptionConnection
                    .Query(x =>
                        (userId == null || x.UserId == userId) &&
                        (forumId == 0 || x.ForumId == forumId) &&
                        (topicId == 0 || x.TopicId == topicId) &&
                        (!isLockedOut))
                    .Select(x => x.Id);

                var query = forumSubscriptionConnection
                    .Query(x => fsQuery.Contains(x.Id))
                    .OrderByDescending(x => x.CreatedOnUtc)
                    .ThenByDescending(x => x.Id);

                return await Task.FromResult(new PagedList<ForumSubscription>(query, pageIndex, pageSize));
            }
        }

        public virtual async Task InsertSubscription(ForumSubscription forumSubscription)
        {
            if (forumSubscription == null)
            {
                throw new ArgumentNullException("forumSubscription");
            }

            await forumSubscriptionRepository.InsertAsync(forumSubscription);

            //event notification
            //_eventPublisher.EntityInserted(forumSubscription);
        }

        public virtual async Task UpdateSubscription(ForumSubscription forumSubscription)
        {
            if (forumSubscription == null)
            {
                throw new ArgumentNullException("forumSubscription");
            }

            await forumSubscriptionRepository.UpdateAsync(forumSubscription);

            //event notification
            //_eventPublisher.EntityUpdated(forumSubscription);
        }

        public virtual async Task<bool> IsUserAllowedToCreateTopic(MantleUser user, Forum forum)
        {
            if (forum == null)
            {
                return false;
            }

            if (user == null)
            {
                return false;
            }

            //if (user.IsGuest() && !forumSettings.AllowGuestsToCreateTopics)
            //{
            //    return false;
            //}

            if (await IsForumModerator(user))
            {
                return true;
            }

            return true;
        }

        public virtual async Task<bool> IsUserAllowedToEditTopic(MantleUser user, ForumTopic topic)
        {
            if (topic == null)
            {
                return false;
            }

            if (user == null)
            {
                return false;
            }

            //if (user.IsGuest())
            //{
            //    return false;
            //}

            if (await IsForumModerator(user))
            {
                return true;
            }

            if (forumSettings.AllowUsersToEditPosts)
            {
                bool ownTopic = user.Id == topic.UserId;
                return ownTopic;
            }

            return false;
        }

        public virtual async Task<bool> IsUserAllowedToMoveTopic(MantleUser user, ForumTopic topic)
        {
            if (topic == null)
            {
                return false;
            }

            if (user == null)
            {
                return false;
            }

            //if (user.IsGuest())
            //{
            //    return false;
            //}

            if (await IsForumModerator(user))
            {
                return true;
            }

            return false;
        }

        public virtual async Task<bool> IsUserAllowedToDeleteTopic(MantleUser user, ForumTopic topic)
        {
            if (topic == null)
            {
                return false;
            }

            if (user == null)
            {
                return false;
            }

            //if (user.IsGuest())
            //{
            //    return false;
            //}

            if (await IsForumModerator(user))
            {
                return true;
            }

            if (forumSettings.AllowUsersToDeletePosts)
            {
                bool ownTopic = user.Id == topic.UserId;
                return ownTopic;
            }

            return false;
        }

        public virtual async Task<bool> IsUserAllowedToCreatePost(MantleUser user, ForumTopic topic)
        {
            if (topic == null)
            {
                return false;
            }

            if (user == null)
            {
                return false;
            }

            //if (user.IsGuest() && !forumSettings.AllowGuestsToCreatePosts)
            //{
            //    return false;
            //}

            return await Task.FromResult(true);
        }

        public virtual async Task<bool> IsUserAllowedToEditPost(MantleUser user, ForumPost post)
        {
            if (post == null)
            {
                return false;
            }

            if (user == null)
            {
                return false;
            }

            //if (user.IsGuest())
            //{
            //    return false;
            //}

            if (await IsForumModerator(user))
            {
                return true;
            }

            if (forumSettings.AllowUsersToEditPosts)
            {
                bool ownPost = user.Id == post.UserId;
                return ownPost;
            }

            return false;
        }

        public virtual async Task<bool> IsUserAllowedToDeletePost(MantleUser user, ForumPost post)
        {
            if (post == null)
            {
                return false;
            }

            if (user == null)
            {
                return false;
            }

            //if (user.IsGuest())
            //{
            //    return false;
            //}

            if (await IsForumModerator(user))
            {
                return true;
            }

            if (forumSettings.AllowUsersToDeletePosts)
            {
                bool ownPost = user.Id == post.UserId;
                return ownPost;
            }

            return false;
        }

        public virtual async Task<bool> IsUserAllowedToSetTopicPriority(MantleUser user)
        {
            if (user == null)
            {
                return false;
            }

            //if (user.IsGuest())
            //{
            //    return false;
            //}

            if (await IsForumModerator(user))
            {
                return true;
            }

            return false;
        }

        public virtual async Task<bool> IsUserAllowedToSubscribe(MantleUser user)
        {
            if (user == null)
            {
                return false;
            }

            //if (user.IsGuest())
            //{
            //    return false;
            //}

            return await Task.FromResult(true);
        }

        public virtual async Task<int> CalculateTopicPageIndex(int forumTopicId, int pageSize, int postId)
        {
            int pageIndex = 0;
            var forumPosts = await GetAllPosts(forumTopicId: forumTopicId, ascSort: true);

            for (int i = 0; i < forumPosts.ItemCount; i++)
            {
                if (forumPosts[i].Id == postId)
                {
                    if (pageSize > 0)
                    {
                        pageIndex = i / pageSize;
                    }
                }
            }

            return pageIndex;
        }

        #endregion Methods

        #region Utilities

        private async Task UpdateForumStats(int forumId)
        {
            if (forumId == 0)
            {
                return;
            }

            var forum = await GetForumById(forumId);
            if (forum == null)
            {
                return;
            }

            using (var forumTopicConnection = forumTopicRepository.OpenConnection())
            using (var forumPostConnection = forumPostRepository.UseConnection(forumTopicConnection))
            {
                //number of topics
                var queryNumTopics = forumTopicConnection.Query(x => x.ForumId == forumId).Select(x => x.Id);

                int numTopics = queryNumTopics.Count();

                //number of posts
                var queryNumPosts = from ft in forumTopicConnection.Query()
                                    join fp in forumPostConnection.Query() on ft.Id equals fp.TopicId
                                    where ft.ForumId == forumId
                                    select fp.Id;

                int numPosts = queryNumPosts.Count();

                //last values
                int lastTopicId = 0;
                int lastPostId = 0;
                string lastPostUserId = null;
                DateTime? lastPostTime = null;

                var queryLastValues = from ft in forumTopicConnection.Query()
                                      join fp in forumPostConnection.Query() on ft.Id equals fp.TopicId
                                      where ft.ForumId == forumId
                                      orderby fp.CreatedOnUtc descending, ft.CreatedOnUtc descending
                                      select new
                                      {
                                          LastTopicId = ft.Id,
                                          LastPostId = fp.Id,
                                          LastPostUserId = fp.UserId,
                                          LastPostTime = fp.CreatedOnUtc
                                      };

                var lastValues = queryLastValues.FirstOrDefault();

                if (lastValues != null)
                {
                    lastTopicId = lastValues.LastTopicId;
                    lastPostId = lastValues.LastPostId;
                    lastPostUserId = lastValues.LastPostUserId;
                    lastPostTime = lastValues.LastPostTime;
                }

                //update forum
                forum.NumTopics = numTopics;
                forum.NumPosts = numPosts;
                forum.LastTopicId = lastTopicId;
                forum.LastPostId = lastPostId;
                forum.LastPostUserId = lastPostUserId;
                forum.LastPostTime = lastPostTime;
            }

            await UpdateForum(forum);
        }

        private async Task UpdateForumTopicStats(int forumTopicId)
        {
            if (forumTopicId == 0)
            {
                return;
            }

            var forumTopic = await GetTopicById(forumTopicId);
            if (forumTopic == null)
            {
                return;
            }

            using (var forumPostConnection = forumPostRepository.OpenConnection())
            {
                //number of posts
                var queryNumPosts = forumPostConnection
                    .Query(x => x.TopicId == forumTopicId)
                    .Select(x => x.Id);

                int numPosts = queryNumPosts.Count();

                //last values
                int lastPostId = 0;
                string lastPostUserId = null;
                DateTime? lastPostTime = null;

                var queryLastValues = forumPostConnection
                    .Query(x => x.TopicId == forumTopicId)
                    .OrderByDescending(x => x.CreatedOnUtc)
                    .Select(x => new
                    {
                        LastPostId = x.Id,
                        LastPostUserId = x.UserId,
                        LastPostTime = x.CreatedOnUtc
                    });

                var lastValues = queryLastValues.FirstOrDefault();
                if (lastValues != null)
                {
                    lastPostId = lastValues.LastPostId;
                    lastPostUserId = lastValues.LastPostUserId;
                    lastPostTime = lastValues.LastPostTime;
                }

                //update topic
                forumTopic.NumPosts = numPosts;
                forumTopic.LastPostId = lastPostId;
                forumTopic.LastPostUserId = lastPostUserId;
                forumTopic.LastPostTime = lastPostTime;
            }

            await UpdateTopic(forumTopic);
        }

        private async Task UpdateUserStats(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var user = await membershipService.GetUserById(userId);

            if (user == null)
            {
                return;
            }

            using (var forumPostConnection = forumPostRepository.OpenConnection())
            {
                int numPosts = await forumPostConnection
                    .Query(x => x.UserId == userId)
                    .Select(x => x.Id)
                    .CountAsync();

                genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.ForumPostCount, numPosts);
            }
        }

        private async Task<bool> IsForumModerator(MantleUser user)
        {
            var roles = await membershipService.GetRolesForUser(user.Id);
            return roles.Any(x => x.Name == Constants.Roles.ForumModerators);
        }

        #endregion Utilities
    }
}