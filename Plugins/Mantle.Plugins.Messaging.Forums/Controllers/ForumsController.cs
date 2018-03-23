using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mantle.Exceptions;
using Mantle.Helpers;
using Mantle.Plugins.Messaging.Forums.Data.Domain;
using Mantle.Plugins.Messaging.Forums.Extensions;
using Mantle.Plugins.Messaging.Forums.Models;
using Mantle.Plugins.Messaging.Forums.Services;
using Mantle.Security.Membership;
using Mantle.Web;
using Mantle.Web.Common.Areas.Admin.Regions.Services;
using Mantle.Web.Configuration;
using Mantle.Web.Configuration.Domain;
using Mantle.Web.Html;
using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SyndicationFeed = WilderMinds.RssSyndication.Feed;
using SyndicationItem = WilderMinds.RssSyndication.Item;

namespace Mantle.Plugins.Messaging.Forums.Controllers
{
    [Authorize]
    [Route("forums")]
    public class ForumsController : MantleController
    {
        #region Fields

        private readonly IForumService forumService;

        //private readonly IPictureService _pictureService;
        private readonly IRegionService regionService;

        private readonly IWebHelper webHelper;
        private readonly ForumSettings forumSettings;

        //private readonly UserSettings _customerSettings;
        //private readonly MediaSettings _mediaSettings;
        private readonly IDateTimeHelper dateTimeHelper;

        private readonly IMembershipService membershipService;
        private readonly SiteSettings siteSettings;

        #endregion Fields

        #region Constructors

        public ForumsController(
            IForumService forumService,
            //IPictureService pictureService,
            IRegionService regionService,
            IWebHelper webHelper,
            ForumSettings forumSettings,
            //UserSettings customerSettings,
            //MediaSettings mediaSettings,
            IDateTimeHelper dateTimeHelper,
            IMembershipService membershipService,
            SiteSettings siteSettings)
        {
            this.forumService = forumService;
            //this._pictureService = pictureService;
            this.regionService = regionService;
            this.webHelper = webHelper;
            this.forumSettings = forumSettings;
            //this._customerSettings = customerSettings;
            //this._mediaSettings = mediaSettings;
            this.dateTimeHelper = dateTimeHelper;
            this.membershipService = membershipService;
            this.siteSettings = siteSettings;
        }

        #endregion Constructors

        #region Utilities

        [NonAction]
        protected virtual async Task<ForumTopicRowModel> PrepareForumTopicRowModel(ForumTopic topic)
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

        [NonAction]
        protected virtual ForumRowModel PrepareForumRowModel(Forum forum)
        {
            var forumRowModel = new ForumRowModel
            {
                Id = forum.Id,
                Name = forum.Name,
                SeName = forum.GetSeName(),
                Description = forum.Description,
                NumTopics = forum.NumTopics,
                NumPosts = forum.NumPosts,
                LastPostId = forum.LastPostId,
            };
            return forumRowModel;
        }

        [NonAction]
        protected virtual async Task<ForumGroupModel> PrepareForumGroupModel(ForumGroup forumGroup)
        {
            var forumGroupModel = new ForumGroupModel
            {
                Id = forumGroup.Id,
                Name = forumGroup.Name,
                SeName = forumGroup.GetSeName(),
            };
            var forums = await forumService.GetAllForumsByGroupId(forumGroup.Id);
            foreach (var forum in forums)
            {
                var forumModel = PrepareForumRowModel(forum);
                forumGroupModel.Forums.Add(forumModel);
            }
            return forumGroupModel;
        }

        [NonAction]
        protected virtual IEnumerable<SelectListItem> ForumTopicTypesList()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem
            {
                Text = T[LocalizableStrings.TopicTypes.Normal],
                Value = ((int)ForumTopicType.Normal).ToString()
            });

            list.Add(new SelectListItem
            {
                Text = T[LocalizableStrings.TopicTypes.Sticky],
                Value = ((int)ForumTopicType.Sticky).ToString()
            });

            list.Add(new SelectListItem
            {
                Text = T[LocalizableStrings.TopicTypes.Announcement],
                Value = ((int)ForumTopicType.Announcement).ToString()
            });

            return list;
        }

        [NonAction]
        protected virtual async Task<IEnumerable<SelectListItem>> ForumGroupsForumsList()
        {
            var forumsSelectList = new List<SelectListItem>();
            var separator = "--";
            var groups = await forumService.GetAllForumGroups();

            foreach (var group in groups)
            {
                // Add the forum group with Value of 0 so it won't be used as a target forum
                forumsSelectList.Add(new SelectListItem { Text = group.Name, Value = "0" });

                var forums = await forumService.GetAllForumsByGroupId(group.Id);
                foreach (var f in forums)
                {
                    forumsSelectList.Add(new SelectListItem { Text = string.Format("{0}{1}", separator, f.Name), Value = f.Id.ToString() });
                }
            }

            return forumsSelectList;
        }

        [NonAction]
        private async Task<bool> IsForumModerator(MantleUser user)
        {
            var roles = await membershipService.GetRolesForUser(user.Id);
            return roles.Any(x => x.Name == Constants.Roles.ForumModerators);
        }

        #endregion Utilities

        #region Methods

        [Route("")]
        public async Task<IActionResult> Index()
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var groups = await forumService.GetAllForumGroups();

            var model = new IndexModel();
            foreach (var group in groups)
            {
                var groupModel = await PrepareForumGroupModel(group);
                model.ForumGroups.Add(groupModel);
            }
            return View(model);
        }

        [Route("active-discussions")]
        [Route("active-discussions/{forumId}/{page}")]
        public async Task<IActionResult> ActiveDiscussions(int forumId = 0, int page = 1)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            int pageSize = forumSettings.ActiveDiscussionsPageSize > 0 ? forumSettings.ActiveDiscussionsPageSize : 50;
            var topics = await forumService.GetActiveTopics(forumId, (page - 1), pageSize);

            var model = new ActiveDiscussionsModel
            {
                TopicPageSize = topics.PageSize,
                TopicTotalRecords = topics.ItemCount,
                TopicPageIndex = topics.PageIndex,
                ViewAllLinkEnabled = false,
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

        ////[ChildActionOnly] // TODO: Convert to ViewComponent?
        //[Route("active-discussions/small")]
        //public IActionResult ActiveDiscussionsSmall()
        //{
        //    if (!forumSettings.ForumsEnabled)
        //    {
        //        return RedirectToHomePage();
        //    }

        //    var topics = AsyncHelper.RunSync(() => forumService.GetActiveTopics(0, 0, forumSettings.HomePageActiveDiscussionsTopicCount));
        //    if (topics.Count == 0)
        //    {
        //        return Content(string.Empty);
        //    }

        //    var model = new ActiveDiscussionsModel
        //    {
        //        ViewAllLinkEnabled = true,
        //        ActiveDiscussionsFeedEnabled = forumSettings.ActiveDiscussionsFeedEnabled,
        //        PostsPageSize = forumSettings.PostsPageSize
        //    };

        //    foreach (var topic in topics)
        //    {
        //        var topicModel = AsyncHelper.RunSync(() => PrepareForumTopicRowModel(topic));
        //        model.ForumTopics.Add(topicModel);
        //    }

        //    return PartialView(model);
        //}

        [Route("active-discussions/rss")]
        [Route("active-discussions/rss/{forumId}")]
        public async Task<IActionResult> ActiveDiscussionsRss(int forumId = 0)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            if (!forumSettings.ActiveDiscussionsFeedEnabled)
            {
                return RedirectToAction("Index");
            }

            var topics = await forumService.GetActiveTopics(forumId, 0, forumSettings.ActiveDiscussionsFeedCount);
            string url = Url.Action("ActiveDiscussionsRSS", "Forums", null, "http");

            string feedTitle = T[LocalizableStrings.ActiveDiscussionsFeedTitle];
            string feedDescription = T[LocalizableStrings.ActiveDiscussionsFeedDescription];

            var feed = new SyndicationFeed
            {
                Title = string.Format(feedTitle, siteSettings.SiteName),
                Description = feedDescription,
                Link = new Uri(url),
                //Id = "ActiveDiscussionsRSS",
                //LastUpdatedTime = DateTime.UtcNow
            };

            var items = new List<SyndicationItem>();

            string viewsText = T[LocalizableStrings.Views];
            string repliesText = T[LocalizableStrings.Replies];

            foreach (var topic in topics)
            {
                string topicUrl = Url.Action("Topic", "Forums", new { id = topic.Id, slug = topic.GetSeName() }, "http");

                string content = string.Format(
                    "{2}: {0}, {3}: {1}",
                    topic.NumReplies.ToString(),
                    topic.Views.ToString(),
                    repliesText,
                    viewsText);

                items.Add(new SyndicationItem
                {
                    Title = topic.Subject,
                    Body = content,
                    Link = new Uri(topicUrl),
                    //Id = string.Format("Topic:{0}", topic.Id),
                    //LastUpdatedTime = (topic.LastPostTime ?? topic.UpdatedOnUtc)
                });
            }
            feed.Items = items;

            return new RssActionResult { Feed = feed };
        }

        [Route("forum-group/{id}/{slug}")]
        public async Task<IActionResult> ForumGroup(int id)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var group = await forumService.GetForumGroupById(id);

            if (group == null)
            {
                return RedirectToAction("Index");
            }

            var model = await PrepareForumGroupModel(group);
            return View(model);
        }

        [Route("forum/{id}/{slug}")]
        [Route("forum/{id}/{slug}/page/{page}")]
        public async Task<IActionResult> Forum(int id, int page = 1)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var forum = await forumService.GetForumById(id);

            if (forum != null)
            {
                int pageSize = forumSettings.TopicsPageSize > 0 ? forumSettings.TopicsPageSize : 10;
                var topics = await forumService.GetAllTopics(forum.Id, null, string.Empty, ForumSearchType.All, 0, (page - 1), pageSize);

                var model = new ForumPageModel
                {
                    Id = forum.Id,
                    Name = forum.Name,
                    SeName = forum.GetSeName(),
                    Description = forum.Description,
                    TopicPageSize = topics.PageSize,
                    TopicTotalRecords = topics.ItemCount,
                    TopicPageIndex = topics.PageIndex,
                    IsUserAllowedToSubscribe = await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser),
                    ForumFeedsEnabled = forumSettings.ForumFeedsEnabled,
                    PostsPageSize = forumSettings.PostsPageSize
                };

                foreach (var topic in topics)
                {
                    var topicModel = await PrepareForumTopicRowModel(topic);
                    model.ForumTopics.Add(topicModel);
                }

                //subscription
                if (await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser))
                {
                    model.WatchForumText = T[LocalizableStrings.WatchForum];

                    var forumSubscription = (await forumService.GetAllSubscriptions(WorkContext.CurrentUser.Id, forum.Id, 0, 0, 1)).FirstOrDefault();
                    if (forumSubscription != null)
                    {
                        model.WatchForumText = T[LocalizableStrings.UnwatchForum];
                    }
                }

                return View(model);
            }

            return RedirectToAction("Index");
        }

        [Route("forum/rss/{id}")]
        public async Task<IActionResult> ForumRss(int id)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            if (!forumSettings.ForumFeedsEnabled)
            {
                return RedirectToAction("Index");
            }

            int topicLimit = forumSettings.ForumFeedCount;
            var forum = await forumService.GetForumById(id);

            if (forum != null)
            {
                //Order by newest topic posts & limit the number of topics to return
                var topics = await forumService.GetAllTopics(forum.Id, null, string.Empty, ForumSearchType.All, 0, 0, topicLimit);

                string url = Url.Action("ForumRSS", "Forums", new { id = forum.Id }, "http");

                string feedTitle = T[LocalizableStrings.ForumFeedTitle];
                string feedDescription = T[LocalizableStrings.ForumFeedDescription];

                var feed = new SyndicationFeed
                {
                    Title = string.Format(feedTitle, siteSettings.SiteName, forum.Name),
                    Description = feedDescription,
                    Link = new Uri(url),
                    //Id = string.Format("ForumRSS:{0}", forum.Id),
                    //LastUpdatedTime = DateTime.UtcNow)
                };

                var items = new List<SyndicationItem>();

                string viewsText = T[LocalizableStrings.Views];
                string repliesText = T[LocalizableStrings.Replies];

                foreach (var topic in topics)
                {
                    string topicUrl = Url.Action("Topic", "Forums", new { id = topic.Id, slug = topic.GetSeName() }, "http");

                    string content = string.Format(
                        "{2}: {0}, {3}: {1}",
                        topic.NumReplies.ToString(),
                        topic.Views.ToString(),
                        repliesText,
                        viewsText);

                    items.Add(new SyndicationItem
                    {
                        Title = topic.Subject,
                        Body = content,
                        Link = new Uri(topicUrl),
                        //Id = string.Format("Topic:{0}", topic.Id),
                        //LastUpdatedTime = (topic.LastPostTime ?? topic.UpdatedOnUtc)
                    });
                }

                feed.Items = items;

                return new RssActionResult { Feed = feed };
            }

            return new RssActionResult { Feed = new SyndicationFeed() };
        }

        [HttpPost]
        [Route("forum/watch/{id}")]
        public async Task<IActionResult> ForumWatch(int id)
        {
            string watchTopic = T[LocalizableStrings.WatchForum];
            string unwatchTopic = T[LocalizableStrings.UnwatchForum];
            string returnText = watchTopic;

            var forum = await forumService.GetForumById(id);
            if (forum == null)
            {
                return Json(new { Subscribed = false, Text = returnText, Error = true });
            }

            if (!await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser))
            {
                return Json(new { Subscribed = false, Text = returnText, Error = true });
            }

            var subscription = (await forumService
                .GetAllSubscriptions(WorkContext.CurrentUser.Id, forum.Id, 0, 0, 1))
                .FirstOrDefault();

            bool subscribed;
            if (subscription == null)
            {
                subscription = new ForumSubscription
                {
                    //SubscriptionGuid = Guid.NewGuid(),
                    UserId = WorkContext.CurrentUser.Id,
                    ForumId = forum.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await forumService.InsertSubscription(subscription);
                subscribed = true;
                returnText = unwatchTopic;
            }
            else
            {
                await forumService.DeleteSubscription(subscription);
                subscribed = false;
            }

            return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
        }

        [Route("topic/{id}/{slug}")]
        [Route("topic/{id}/{slug}/page/{page}")]
        public async Task<IActionResult> Topic(int id, int page = 1)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var topic = await forumService.GetTopicById(id);

            if (topic != null)
            {
                //load posts
                var posts = await forumService.GetAllPosts(topic.Id, null, string.Empty, page - 1, forumSettings.PostsPageSize);

                //if not posts loaded, redirect to the first page
                if (posts.Count == 0 && page > 1)
                {
                    return RedirectToAction("Topic", new { id = topic.Id, slug = topic.GetSeName() });
                }

                //update view count
                topic.Views += 1;
                await forumService.UpdateTopic(topic);

                //prepare model
                var model = new ForumTopicPageModel
                {
                    Id = topic.Id,
                    Subject = topic.Subject,
                    SeName = topic.GetSeName(),
                    IsUserAllowedToEditTopic = await forumService.IsUserAllowedToEditTopic(WorkContext.CurrentUser, topic),
                    IsUserAllowedToDeleteTopic = await forumService.IsUserAllowedToDeleteTopic(WorkContext.CurrentUser, topic),
                    IsUserAllowedToMoveTopic = await forumService.IsUserAllowedToMoveTopic(WorkContext.CurrentUser, topic),
                    IsUserAllowedToSubscribe = await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser),
                    PostsPageIndex = posts.PageIndex,
                    PostsPageSize = posts.PageSize,
                    PostsTotalRecords = posts.ItemCount
                };

                if (model.IsUserAllowedToSubscribe)
                {
                    model.WatchTopicText = T[LocalizableStrings.WatchTopic];

                    var forumTopicSubscription = (await forumService
                        .GetAllSubscriptions(WorkContext.CurrentUser.Id, 0, topic.Id, 0, 1))
                        .FirstOrDefault();

                    if (forumTopicSubscription != null)
                    {
                        model.WatchTopicText = T[LocalizableStrings.UnwatchTopic];
                    }
                }

                foreach (var post in posts)
                {
                    var postUser = await membershipService.GetUserById(post.UserId);

                    var postModel = new ForumPostModel
                    {
                        Id = post.Id,
                        ForumTopicId = post.TopicId,
                        ForumTopicSeName = topic.GetSeName(),
                        FormattedText = post.FormatPostText(),
                        IsCurrentUserAllowedToEditPost = await forumService.IsUserAllowedToEditPost(WorkContext.CurrentUser, post),
                        IsCurrentUserAllowedToDeletePost = await forumService.IsUserAllowedToDeletePost(WorkContext.CurrentUser, post),
                        UserId = post.UserId,
                        //AllowViewingProfiles = _customerSettings.AllowViewingProfiles, //TODO
                        AllowViewingProfiles = false,
                        UserName = await membershipService.GetUserDisplayName(postUser),
                        IsUserForumModerator = await IsForumModerator(postUser),
                        ShowUsersPostCount = forumSettings.ShowUsersPostCount,
                        ForumPostCount = postUser.GetAttribute<int>(SystemUserAttributeNames.ForumPostCount),
                        //ShowUsersJoinDate = _customerSettings.ShowUsersJoinDate, // TODO
                        ShowUsersJoinDate = false,
                        //UserJoinDate = postUser.CreatedOnUtc, //TODO
                        AllowPrivateMessages = forumSettings.AllowPrivateMessages,
                        SignaturesEnabled = forumSettings.SignaturesEnabled,
                        FormattedSignature = postUser.GetAttribute<string>(SystemUserAttributeNames.Signature).FormatForumSignatureText(),
                    };
                    //created on string
                    if (forumSettings.RelativeDateTimeFormattingEnabled)
                    {
                        postModel.PostCreatedOnStr = post.CreatedOnUtc.RelativeFormat(true, "f");
                    }
                    else
                    {
                        postModel.PostCreatedOnStr = dateTimeHelper.ConvertToUserTime(post.CreatedOnUtc, DateTimeKind.Utc).ToString("f");
                    }

                    // TODO
                    ////avatar
                    //if (_customerSettings.AllowUsersToUploadAvatars)
                    //{
                    //    postModel.UserAvatarUrl = _pictureService.GetPictureUrl(
                    //        postUser.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId),
                    //        _mediaSettings.AvatarPictureSize,
                    //        _customerSettings.DefaultAvatarEnabled,
                    //        defaultPictureType: PictureType.Avatar);
                    //}

                    ////location
                    //postModel.ShowUsersLocation = _customerSettings.ShowUsersLocation;
                    //if (_customerSettings.ShowUsersLocation)
                    //{
                    //    var countryId = postUser.GetAttribute<int>(SystemUserAttributeNames.CountryId);
                    //    var country = regionService.FindOne(countryId);
                    //    postModel.UserLocation = country != null ? country.GetLocalized(x => x.Name) : string.Empty;
                    //}

                    // page number is needed for creating post link in _ForumPost partial view
                    postModel.CurrentTopicPage = page;
                    model.ForumPostModels.Add(postModel);
                }

                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("topic/watch/{id}")]
        public async Task<IActionResult> TopicWatch(int id)
        {
            string watchTopic = T[LocalizableStrings.WatchTopic];
            string unwatchTopic = T[LocalizableStrings.UnwatchTopic];
            string returnText = watchTopic;

            var topic = await forumService.GetTopicById(id);
            if (topic == null)
            {
                return Json(new { Subscribed = false, Text = returnText, Error = true });
            }

            if (!await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser))
            {
                return Json(new { Subscribed = false, Text = returnText, Error = true });
            }

            var subscription = (await forumService
                .GetAllSubscriptions(WorkContext.CurrentUser.Id, 0, topic.Id, 0, 1))
                .FirstOrDefault();

            bool subscribed;
            if (subscription == null)
            {
                subscription = new ForumSubscription
                {
                    //SubscriptionGuid = Guid.NewGuid(),
                    UserId = WorkContext.CurrentUser.Id,
                    TopicId = topic.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };
                await forumService.InsertSubscription(subscription);
                subscribed = true;
                returnText = unwatchTopic;
            }
            else
            {
                await forumService.DeleteSubscription(subscription);
                subscribed = false;
            }

            return Json(new { Subscribed = subscribed, Text = returnText, Error = false });
        }

        [Route("topic/move/{id}")]
        public async Task<IActionResult> TopicMove(int id)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var topic = await forumService.GetTopicById(id);
            if (topic == null)
            {
                return RedirectToAction("Index");
            }

            var model = new TopicMoveModel
            {
                ForumList = await ForumGroupsForumsList(),
                Id = topic.Id,
                TopicSeName = topic.GetSeName(),
                ForumSelected = topic.ForumId
            };

            return View(model);
        }

        [HttpPost]
        //[FrontendAntiForgery]
        [Route("topic/move-post")]
        public async Task<IActionResult> TopicMovePost(TopicMoveModel model)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var topic = await forumService.GetTopicById(model.Id);
            if (topic == null)
            {
                return RedirectToAction("Index");
            }

            int newForumId = model.ForumSelected;
            var forum = await forumService.GetForumById(newForumId);

            if (forum != null && topic.ForumId != newForumId)
            {
                await forumService.MoveTopic(topic.Id, newForumId);
            }

            return RedirectToAction("Topic", new { id = topic.Id, slug = topic.GetSeName() });
        }

        [Route("topic/delete/{id}")]
        public async Task<IActionResult> TopicDelete(int id)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var topic = await forumService.GetTopicById(id);
            if (topic != null)
            {
                if (!await forumService.IsUserAllowedToDeleteTopic(WorkContext.CurrentUser, topic))
                {
                    return Unauthorized();
                }
                var forum = await forumService.GetForumById(topic.ForumId);

                await forumService.DeleteTopic(topic);

                if (forum != null)
                {
                    return RedirectToAction("Forum", new { id = forum.Id, slug = forum.GetSeName() });
                }
            }

            return RedirectToAction("Index");
        }

        [Route("topic/create/{id}")]
        public async Task<IActionResult> TopicCreate(int id)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var forum = await forumService.GetForumById(id);
            if (forum == null)
            {
                return RedirectToAction("Index");
            }

            if (await forumService.IsUserAllowedToCreateTopic(WorkContext.CurrentUser, forum) == false)
            {
                return Unauthorized();
            }

            var model = new EditForumTopicModel
            {
                Id = 0,
                IsEdit = false,
                ForumId = forum.Id,
                ForumName = forum.Name,
                ForumSeName = forum.GetSeName(),
                ForumEditor = forumSettings.ForumEditor,
                IsUserAllowedToSetTopicPriority = await forumService.IsUserAllowedToSetTopicPriority(WorkContext.CurrentUser),
                TopicPriorities = ForumTopicTypesList(),
                IsUserAllowedToSubscribe = await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser),
                Subscribed = false
            };
            return View(model);
        }

        [HttpPost]
        //[FrontendAntiForgery]
        [Route("topic/create-post")]
        //[ValidateInput(false)]
        public async Task<IActionResult> TopicCreatePost(EditForumTopicModel model)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var forum = await forumService.GetForumById(model.ForumId);
            if (forum == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await forumService.IsUserAllowedToCreateTopic(WorkContext.CurrentUser, forum))
                    {
                        return Unauthorized();
                    }

                    string subject = model.Subject;
                    int maxSubjectLength = forumSettings.TopicSubjectMaxLength;

                    if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                    {
                        subject = subject.Substring(0, maxSubjectLength);
                    }

                    string text = model.Text;
                    int maxPostLength = forumSettings.PostMaxLength;

                    if (maxPostLength > 0 && text.Length > maxPostLength)
                    {
                        text = text.Substring(0, maxPostLength);
                    }

                    var topicType = ForumTopicType.Normal;

                    string ipAddress = webHelper.GetRemoteIpAddress();

                    var nowUtc = DateTime.UtcNow;

                    if (await forumService.IsUserAllowedToSetTopicPriority(WorkContext.CurrentUser))
                    {
                        topicType = model.TopicType;
                    }

                    //forum topic
                    var topic = new ForumTopic
                    {
                        ForumId = forum.Id,
                        UserId = WorkContext.CurrentUser.Id,
                        TopicType = topicType,
                        Subject = subject,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    await forumService.InsertTopic(topic, true);

                    //forum post
                    var post = new ForumPost
                    {
                        TopicId = topic.Id,
                        UserId = WorkContext.CurrentUser.Id,
                        Text = text,
                        IPAddress = ipAddress,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    await forumService.InsertPost(post, false);

                    //update forum topic
                    topic.NumPosts = 1;
                    topic.LastPostId = post.Id;
                    topic.LastPostUserId = post.UserId;
                    topic.LastPostTime = post.CreatedOnUtc;
                    topic.UpdatedOnUtc = nowUtc;
                    await forumService.UpdateTopic(topic);

                    //subscription
                    if (await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser))
                    {
                        if (model.Subscribed)
                        {
                            var forumSubscription = new ForumSubscription
                            {
                                //SubscriptionGuid = Guid.NewGuid(),
                                UserId = WorkContext.CurrentUser.Id,
                                TopicId = topic.Id,
                                CreatedOnUtc = nowUtc
                            };

                            await forumService.InsertSubscription(forumSubscription);
                        }
                    }

                    return RedirectToAction("Topic", new { id = topic.Id, slug = topic.GetSeName() });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            // redisplay form
            model.TopicPriorities = ForumTopicTypesList();
            model.IsEdit = false;
            model.ForumId = forum.Id;
            model.ForumName = forum.Name;
            model.ForumSeName = forum.GetSeName();
            model.Id = 0;
            model.IsUserAllowedToSetTopicPriority = await forumService.IsUserAllowedToSetTopicPriority(WorkContext.CurrentUser);
            model.IsUserAllowedToSubscribe = await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser);
            model.ForumEditor = forumSettings.ForumEditor;

            return View("TopicCreate", model);
        }

        [Route("topic/edit/{id}")]
        public async Task<IActionResult> TopicEdit(int id)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var topic = await forumService.GetTopicById(id);
            if (topic == null)
            {
                return RedirectToAction("Index");
            }

            if (!await forumService.IsUserAllowedToEditTopic(WorkContext.CurrentUser, topic))
            {
                return Unauthorized();
            }

            var forum = topic.Forum ?? await forumService.GetForumById(topic.ForumId);
            if (forum == null)
            {
                return RedirectToAction("Index");
            }

            var firstPost = topic.GetFirstPost(forumService);

            var model = new EditForumTopicModel
            {
                IsEdit = true,
                TopicPriorities = ForumTopicTypesList(),
                ForumName = forum.Name,
                ForumSeName = forum.GetSeName(),
                Text = firstPost.Text,
                Subject = topic.Subject,
                TopicType = topic.TopicType,
                Id = topic.Id,
                ForumId = forum.Id,
                ForumEditor = forumSettings.ForumEditor,
                IsUserAllowedToSetTopicPriority = await forumService.IsUserAllowedToSetTopicPriority(WorkContext.CurrentUser),
                IsUserAllowedToSubscribe = await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser)
            };

            //subscription
            if (model.IsUserAllowedToSubscribe)
            {
                var subscription = (await forumService
                    .GetAllSubscriptions(WorkContext.CurrentUser.Id, 0, topic.Id, 0, 1))
                    .FirstOrDefault();

                model.Subscribed = subscription != null;
            }

            return View(model);
        }

        [HttpPost]
        //[FrontendAntiForgery]
        [Route("topic/edit-post")]
        //[ValidateInput(false)]
        public async Task<IActionResult> TopicEditPost(EditForumTopicModel model)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var topic = await forumService.GetTopicById(model.Id);
            if (topic == null)
            {
                return RedirectToAction("Index");
            }

            var forum = topic.Forum ?? await forumService.GetForumById(topic.ForumId);
            if (forum == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await forumService.IsUserAllowedToEditTopic(WorkContext.CurrentUser, topic))
                    {
                        return Unauthorized();
                    }

                    string subject = model.Subject;
                    int maxSubjectLength = forumSettings.TopicSubjectMaxLength;

                    if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                    {
                        subject = subject.Substring(0, maxSubjectLength);
                    }

                    string text = model.Text;
                    int maxPostLength = forumSettings.PostMaxLength;

                    if (maxPostLength > 0 && text.Length > maxPostLength)
                    {
                        text = text.Substring(0, maxPostLength);
                    }

                    var topicType = ForumTopicType.Normal;

                    string ipAddress = webHelper.GetRemoteIpAddress();

                    DateTime nowUtc = DateTime.UtcNow;

                    if (await forumService.IsUserAllowedToSetTopicPriority(WorkContext.CurrentUser))
                    {
                        topicType = model.TopicType;
                    }

                    //forum topic
                    topic.TopicType = topicType;
                    topic.Subject = subject;
                    topic.UpdatedOnUtc = nowUtc;
                    await forumService.UpdateTopic(topic);

                    //forum post
                    var firstPost = topic.GetFirstPost(forumService);
                    if (firstPost != null)
                    {
                        firstPost.Text = text;
                        firstPost.UpdatedOnUtc = nowUtc;
                        await forumService.UpdatePost(firstPost);
                    }
                    else
                    {
                        //error (not possible)
                        firstPost = new ForumPost
                        {
                            TopicId = topic.Id,
                            UserId = topic.UserId,
                            Text = text,
                            IPAddress = ipAddress,
                            UpdatedOnUtc = nowUtc
                        };

                        await forumService.InsertPost(firstPost, false);
                    }

                    //subscription
                    if (await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser))
                    {
                        var subscription = (await forumService
                            .GetAllSubscriptions(WorkContext.CurrentUser.Id, 0, topic.Id, 0, 1))
                            .FirstOrDefault();

                        if (model.Subscribed)
                        {
                            if (subscription == null)
                            {
                                subscription = new ForumSubscription
                                {
                                    //SubscriptionGuid = Guid.NewGuid(),
                                    UserId = WorkContext.CurrentUser.Id,
                                    TopicId = topic.Id,
                                    CreatedOnUtc = nowUtc
                                };

                                await forumService.InsertSubscription(subscription);
                            }
                        }
                        else
                        {
                            if (subscription != null)
                            {
                                await forumService.DeleteSubscription(subscription);
                            }
                        }
                    }

                    // redirect to the topic page with the topic slug
                    return RedirectToAction("Topic", new { id = topic.Id, slug = topic.GetSeName() });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            // redisplay form
            model.TopicPriorities = ForumTopicTypesList();
            model.IsEdit = true;
            model.ForumName = forum.Name;
            model.ForumSeName = forum.GetSeName();
            model.ForumId = forum.Id;
            model.ForumEditor = forumSettings.ForumEditor;

            model.IsUserAllowedToSetTopicPriority = await forumService.IsUserAllowedToSetTopicPriority(WorkContext.CurrentUser);
            model.IsUserAllowedToSubscribe = await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser);

            return View("TopicEdit", model);
        }

        [HttpPost]
        //[FrontendAntiForgery]
        [Route("topic/save")]
        //[ValidateInput(false)]
        public async Task<IActionResult> TopicSave(EditForumTopicModel model)
        {
            if (model.IsEdit)
            {
                return await TopicEditPost(model);
            }
            else
            {
                return await TopicCreatePost(model);
            }
        }

        [Route("post/delete/{id}")]
        public async Task<IActionResult> PostDelete(int id)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var post = await forumService.GetPostById(id);
            if (post != null)
            {
                if (!await forumService.IsUserAllowedToDeletePost(WorkContext.CurrentUser, post))
                {
                    return Unauthorized();
                }

                var topic = post.ForumTopic ?? await forumService.GetTopicById(post.TopicId);
                var forum = topic.Forum ?? await forumService.GetForumById(topic.ForumId);

                int forumId = forum.Id;
                string forumSlug = forum.GetSeName();

                await forumService.DeletePost(post);

                //get topic one more time because it can be deleted (first or only post deleted)
                topic = await forumService.GetTopicById(post.TopicId);
                if (topic == null)
                {
                    return RedirectToAction("Forum", new { id = forumId, slug = forumSlug });
                }
                return RedirectToAction("Topic", new { id = topic.Id, slug = topic.GetSeName() });
            }

            return RedirectToAction("Index");
        }

        [Route("post/create/{id}/{quote?}")]
        public async Task<IActionResult> PostCreate(int id, int? quote)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var topic = await forumService.GetTopicById(id);
            if (topic == null)
            {
                return RedirectToAction("Index");
            }

            if (!await forumService.IsUserAllowedToCreatePost(WorkContext.CurrentUser, topic))
            {
                return Unauthorized();
            }

            var forum = topic.Forum ?? await forumService.GetForumById(topic.ForumId);
            if (forum == null)
            {
                return RedirectToAction("Index");
            }

            var model = new EditForumPostModel
            {
                Id = 0,
                ForumTopicId = topic.Id,
                IsEdit = false,
                ForumEditor = forumSettings.ForumEditor,
                ForumName = forum.Name,
                ForumTopicSubject = topic.Subject,
                ForumTopicSeName = topic.GetSeName(),
                IsUserAllowedToSubscribe = await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser),
                Subscribed = false,
            };

            //subscription
            if (model.IsUserAllowedToSubscribe)
            {
                var subscription = (await forumService
                    .GetAllSubscriptions(WorkContext.CurrentUser.Id, 0, topic.Id, 0, 1))
                    .FirstOrDefault();

                model.Subscribed = subscription != null;
            }

            // Insert the quoted text
            string text = string.Empty;
            if (quote.HasValue)
            {
                var quotePost = await forumService.GetPostById(quote.Value);
                var quotePostUser = await membershipService.GetUserById(quotePost.UserId);

                if (quotePost != null && quotePost.TopicId == topic.Id)
                {
                    switch (forumSettings.ForumEditor)
                    {
                        case EditorType.SimpleTextBox:
                            text = string.Format(
                                "{0}:\n{1}\n",
                                await membershipService.GetUserDisplayName(quotePostUser),
                                quotePost.Text);
                            break;

                        case EditorType.BBCodeEditor:
                            text = string.Format(
                                "[quote={0}]{1}[/quote]",
                                await membershipService.GetUserDisplayName(quotePostUser),
                                BBCodeHelper.RemoveQuotes(quotePost.Text));
                            break;
                    }
                    model.Text = text;
                }
            }

            return View(model);
        }

        [HttpPost]
        //[FrontendAntiForgery]
        [Route("post/create-post")]
        //[ValidateInput(false)]
        public async Task<IActionResult> PostCreatePost(EditForumPostModel model)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var topic = await forumService.GetTopicById(model.ForumTopicId);
            if (topic == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await forumService.IsUserAllowedToCreatePost(WorkContext.CurrentUser, topic))
                    {
                        return Unauthorized();
                    }

                    string text = model.Text;
                    var maxPostLength = forumSettings.PostMaxLength;

                    if (maxPostLength > 0 && text.Length > maxPostLength)
                    {
                        text = text.Substring(0, maxPostLength);
                    }

                    string ipAddress = webHelper.GetRemoteIpAddress();

                    DateTime nowUtc = DateTime.UtcNow;

                    var post = new ForumPost
                    {
                        TopicId = topic.Id,
                        UserId = WorkContext.CurrentUser.Id,
                        Text = text,
                        IPAddress = ipAddress,
                        CreatedOnUtc = nowUtc,
                        UpdatedOnUtc = nowUtc
                    };
                    await forumService.InsertPost(post, true);

                    //subscription
                    if (await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser))
                    {
                        var forumSubscription = (await forumService
                            .GetAllSubscriptions(WorkContext.CurrentUser.Id, 0, post.TopicId, 0, 1))
                            .FirstOrDefault();

                        if (model.Subscribed)
                        {
                            if (forumSubscription == null)
                            {
                                forumSubscription = new ForumSubscription
                                {
                                    //SubscriptionGuid = Guid.NewGuid(),
                                    UserId = WorkContext.CurrentUser.Id,
                                    TopicId = post.TopicId,
                                    CreatedOnUtc = nowUtc
                                };

                                await forumService.InsertSubscription(forumSubscription);
                            }
                        }
                        else
                        {
                            if (forumSubscription != null)
                            {
                                await forumService.DeleteSubscription(forumSubscription);
                            }
                        }
                    }

                    int pageSize = forumSettings.PostsPageSize > 0 ? forumSettings.PostsPageSize : 10;

                    int pageIndex = (await forumService.CalculateTopicPageIndex(post.TopicId, pageSize, post.Id) + 1);
                    string url = string.Empty;

                    if (pageIndex > 1)
                    {
                        url = Url.Action("Topic", new { id = post.TopicId, slug = topic.GetSeName(), page = pageIndex });
                    }
                    else
                    {
                        url = Url.Action("Topic", new { id = post.TopicId, slug = topic.GetSeName() });
                    }
                    return Redirect(string.Format("{0}#{1}", url, post.Id));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            // redisplay form
            var forum = topic.Forum ?? await forumService.GetForumById(topic.ForumId);
            if (forum == null)
            {
                return RedirectToAction("Index");
            }

            model.IsEdit = false;
            model.ForumName = forum.Name;
            model.ForumTopicId = topic.Id;
            model.ForumTopicSubject = topic.Subject;
            model.ForumTopicSeName = topic.GetSeName();
            model.Id = 0;
            model.IsUserAllowedToSubscribe = await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser);
            model.ForumEditor = forumSettings.ForumEditor;

            return View("PostCreate", model);
        }

        [Route("post/edit/{id}")]
        public async Task<IActionResult> PostEdit(int id)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var post = await forumService.GetPostById(id);
            if (post == null)
            {
                return RedirectToAction("Index");
            }
            if (!await forumService.IsUserAllowedToEditPost(WorkContext.CurrentUser, post))
            {
                return Unauthorized();
            }

            var topic = post.ForumTopic ?? await forumService.GetTopicById(post.TopicId);
            if (topic == null)
            {
                return RedirectToAction("Index");
            }

            var forum = topic.Forum ?? await forumService.GetForumById(topic.ForumId);
            if (forum == null)
            {
                return RedirectToAction("Index");
            }

            var model = new EditForumPostModel
            {
                Id = post.Id,
                ForumTopicId = topic.Id,
                IsEdit = true,
                ForumEditor = forumSettings.ForumEditor,
                ForumName = forum.Name,
                ForumTopicSubject = topic.Subject,
                ForumTopicSeName = topic.GetSeName(),
                IsUserAllowedToSubscribe = await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser),
                Subscribed = false,
                Text = post.Text,
            };

            //subscription
            if (model.IsUserAllowedToSubscribe)
            {
                var subscription = (await forumService
                    .GetAllSubscriptions(WorkContext.CurrentUser.Id, 0, topic.Id, 0, 1))
                    .FirstOrDefault();

                model.Subscribed = subscription != null;
            }

            return View(model);
        }

        [HttpPost]
        //[FrontendAntiForgery]
        [Route("post/edit-post")]
        //[ValidateInput(false)]
        public async Task<IActionResult> PostEditPost(EditForumPostModel model)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            var post = await forumService.GetPostById(model.Id);
            if (post == null)
            {
                return RedirectToAction("Index");
            }

            if (!await forumService.IsUserAllowedToEditPost(WorkContext.CurrentUser, post))
            {
                return Unauthorized();
            }

            var topic = post.ForumTopic ?? await forumService.GetTopicById(post.TopicId);
            if (topic == null)
            {
                return RedirectToAction("Index");
            }

            var forum = topic.Forum ?? await forumService.GetForumById(topic.ForumId);
            if (forum == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var nowUtc = DateTime.UtcNow;

                    string text = model.Text;
                    var maxPostLength = forumSettings.PostMaxLength;
                    if (maxPostLength > 0 && text.Length > maxPostLength)
                    {
                        text = text.Substring(0, maxPostLength);
                    }

                    post.UpdatedOnUtc = nowUtc;
                    post.Text = text;
                    await forumService.UpdatePost(post);

                    //subscription
                    if (await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser))
                    {
                        var subscription = (await forumService
                            .GetAllSubscriptions(WorkContext.CurrentUser.Id, 0, post.TopicId, 0, 1))
                            .FirstOrDefault();

                        if (model.Subscribed)
                        {
                            if (subscription == null)
                            {
                                subscription = new ForumSubscription
                                {
                                    //SubscriptionGuid = Guid.NewGuid(),
                                    UserId = WorkContext.CurrentUser.Id,
                                    TopicId = post.TopicId,
                                    CreatedOnUtc = nowUtc
                                };
                                await forumService.InsertSubscription(subscription);
                            }
                        }
                        else
                        {
                            if (subscription != null)
                            {
                                await forumService.DeleteSubscription(subscription);
                            }
                        }
                    }

                    int pageSize = forumSettings.PostsPageSize > 0 ? forumSettings.PostsPageSize : 10;
                    int pageIndex = (await forumService.CalculateTopicPageIndex(post.TopicId, pageSize, post.Id) + 1);
                    var url = string.Empty;
                    if (pageIndex > 1)
                    {
                        url = Url.Action("Topic", new { id = post.TopicId, slug = topic.GetSeName(), page = pageIndex });
                    }
                    else
                    {
                        url = Url.Action("Topic", new { id = post.TopicId, slug = topic.GetSeName() });
                    }
                    return Redirect(string.Format("{0}#{1}", url, post.Id));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            //redisplay form
            model.IsEdit = true;
            model.ForumName = forum.Name;
            model.ForumTopicId = topic.Id;
            model.ForumTopicSubject = topic.Subject;
            model.ForumTopicSeName = topic.GetSeName();
            model.Id = post.Id;
            model.IsUserAllowedToSubscribe = await forumService.IsUserAllowedToSubscribe(WorkContext.CurrentUser);
            model.ForumEditor = forumSettings.ForumEditor;

            return View("PostEdit", model);
        }

        [HttpPost]
        //[FrontendAntiForgery]
        [Route("post/save")]
        //[ValidateInput(false)]
        public async Task<IActionResult> PostSave(EditForumPostModel model)
        {
            if (model.IsEdit)
            {
                return await PostEditPost(model);
            }
            else
            {
                return await PostCreatePost(model);
            }
        }

        [Route("search")]
        public async Task<IActionResult> Search(
            string searchterms,
            bool? adv,
            string forumId,
            string within,
            string limitDays,
            int page = 1)
        {
            if (!forumSettings.ForumsEnabled)
            {
                return RedirectToHomePage();
            }

            int pageSize = 10;

            var model = new SearchModel();

            // Create the values for the "Limit results to previous" select list
            var limitList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = T[LocalizableStrings.Models.Search.LimitResultsToPrevious.AllResults],
                    Value = "0"
                },
                new SelectListItem
                {
                    Text = T[LocalizableStrings.Models.Search.LimitResultsToPrevious.OneDay],
                    Value = "1"
                },
                new SelectListItem
                {
                    Text = T[LocalizableStrings.Models.Search.LimitResultsToPrevious.SevenDays],
                    Value = "7"
                },
                new SelectListItem
                {
                    Text = T[LocalizableStrings.Models.Search.LimitResultsToPrevious.TwoWeeks],
                    Value = "14"
                },
                new SelectListItem
                {
                    Text = T[LocalizableStrings.Models.Search.LimitResultsToPrevious.OneMonth],
                    Value = "30"
                },
                new SelectListItem
                {
                    Text = T[LocalizableStrings.Models.Search.LimitResultsToPrevious.ThreeMonths],
                    Value = "92"
                },
                new SelectListItem
                {
                    Text= T[LocalizableStrings.Models.Search.LimitResultsToPrevious.SixMonths],
                    Value = "183"
                },
                new SelectListItem
                {
                    Text = T[LocalizableStrings.Models.Search.LimitResultsToPrevious.OneYear],
                    Value = "365"
                }
            };
            model.LimitList = limitList;

            // Create the values for the "Search in forum" select list
            var forumsSelectList = new List<SelectListItem>();

            forumsSelectList.Add(
                new SelectListItem
                {
                    Text = T[LocalizableStrings.Models.Search.SearchInForum.All],
                    Value = "0",
                    Selected = true,
                });

            string separator = "--";

            var groups = await forumService.GetAllForumGroups();
            foreach (var group in groups)
            {
                // Add the forum group with value as '-' so it can't be used as a target forum id
                forumsSelectList.Add(new SelectListItem { Text = group.Name, Value = "-" });

                var forums = await forumService.GetAllForumsByGroupId(group.Id);
                foreach (var forum in forums)
                {
                    forumsSelectList.Add(new SelectListItem
                    {
                        Text = string.Format("{0}{1}", separator, forum.Name),
                        Value = forum.Id.ToString()
                    });
                }
            }
            model.ForumList = forumsSelectList;

            // Create the values for "Search within" select list
            var withinList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = ((int)ForumSearchType.All).ToString(),
                    Text = T[LocalizableStrings.Models.Search.SearchWithin.All]
                },
                new SelectListItem
                {
                    Value = ((int)ForumSearchType.TopicTitlesOnly).ToString(),
                    Text = T[LocalizableStrings.Models.Search.SearchWithin.TopicTitlesOnly]
                },
                new SelectListItem
                {
                    Value = ((int)ForumSearchType.PostTextOnly).ToString(),
                    Text = T[LocalizableStrings.Models.Search.SearchWithin.PostTextOnly]
                }
            };
            model.WithinList = withinList;

            int forumIdSelected;
            int.TryParse(forumId, out forumIdSelected);
            model.ForumIdSelected = forumIdSelected;

            int withinSelected;
            int.TryParse(within, out withinSelected);
            model.WithinSelected = withinSelected;

            int limitDaysSelected;
            int.TryParse(limitDays, out limitDaysSelected);
            model.LimitDaysSelected = limitDaysSelected;

            int searchTermMinimumLength = forumSettings.ForumSearchTermMinimumLength;

            model.ShowAdvancedSearch = adv.GetValueOrDefault();
            model.SearchResultsVisible = false;
            model.NoResultsVisisble = false;
            model.PostsPageSize = forumSettings.PostsPageSize;

            try
            {
                if (!string.IsNullOrWhiteSpace(searchterms))
                {
                    searchterms = searchterms.Trim();
                    model.SearchTerms = searchterms;

                    if (searchterms.Length < searchTermMinimumLength)
                    {
                        throw new MantleException(string.Format(
                            T[LocalizableStrings.SearchTermMinimumLengthIsNCharacters],
                            searchTermMinimumLength));
                    }

                    ForumSearchType searchWithin = 0;
                    int limitResultsToPrevious = 0;
                    if (adv.GetValueOrDefault())
                    {
                        searchWithin = (ForumSearchType)withinSelected;
                        limitResultsToPrevious = limitDaysSelected;
                    }

                    if (forumSettings.SearchResultsPageSize > 0)
                    {
                        pageSize = forumSettings.SearchResultsPageSize;
                    }

                    var topics = await forumService.GetAllTopics(
                        forumIdSelected,
                        null,
                        searchterms,
                        searchWithin,
                        limitResultsToPrevious,
                        page - 1,
                        pageSize);

                    model.TopicPageSize = topics.PageSize;
                    model.TopicTotalRecords = topics.ItemCount;
                    model.TopicPageIndex = topics.PageIndex;

                    foreach (var topic in topics)
                    {
                        var topicModel = await PrepareForumTopicRowModel(topic);
                        model.ForumTopics.Add(topicModel);
                    }

                    model.SearchResultsVisible = (topics.Count > 0);
                    model.NoResultsVisisble = !(model.SearchResultsVisible);

                    return View(model);
                }
                model.SearchResultsVisible = false;
            }
            catch (Exception ex)
            {
                model.Error = ex.Message;
            }

            //some exception raised
            model.TopicPageSize = pageSize;
            model.TopicTotalRecords = 0;
            model.TopicPageIndex = page - 1;

            return View(model);
        }

        ////[ChildActionOnly] // TODO: Convert to ViewComponent?
        //[Route("last-post/{forumPostId}/{showTopic}")]
        //public IActionResult LastPost(int forumPostId, bool showTopic)
        //{
        //    var post = AsyncHelper.RunSync(() => forumService.GetPostById(forumPostId));
        //    var model = new LastPostModel();

        //    if (post != null)
        //    {
        //        var postUser = AsyncHelper.RunSync(() => membershipService.GetUserById(post.UserId));
        //        var topic = post.ForumTopic ?? AsyncHelper.RunSync(() => forumService.GetTopicById(post.TopicId));

        //        model.Id = post.Id;
        //        model.ForumTopicId = post.TopicId;
        //        model.ForumTopicSeName = topic.GetSeName();
        //        model.ForumTopicSubject = topic.StripTopicSubject();
        //        model.UserId = post.UserId;
        //        //model.AllowViewingProfiles = _customerSettings.AllowViewingProfiles; //TODO
        //        model.AllowViewingProfiles = true;
        //        model.UserName = AsyncHelper.RunSync(() => membershipService.GetUserDisplayName(postUser));
        //        //created on string
        //        if (forumSettings.RelativeDateTimeFormattingEnabled)
        //        {
        //            model.PostCreatedOnStr = post.CreatedOnUtc.RelativeFormat(true, "f");
        //        }
        //        else
        //        {
        //            model.PostCreatedOnStr = dateTimeHelper.ConvertToUserTime(post.CreatedOnUtc, DateTimeKind.Utc).ToString("f");
        //        }
        //    }
        //    model.ShowTopic = showTopic;
        //    return PartialView(model);
        //}

        ////[ChildActionOnly] // TODO: Convert to ViewComponent?
        //[Route("breadcrumb")]
        //public IActionResult ForumBreadcrumb(int? forumGroupId, int? forumId, int? forumTopicId)
        //{
        //    var model = new ForumBreadcrumbModel();

        //    ForumTopic topic = null;
        //    if (forumTopicId.HasValue)
        //    {
        //        topic = AsyncHelper.RunSync(() => forumService.GetTopicById(forumTopicId.Value));
        //        if (topic != null)
        //        {
        //            model.ForumTopicId = topic.Id;
        //            model.ForumTopicSubject = topic.Subject;
        //            model.ForumTopicSeName = topic.GetSeName();
        //        }
        //    }

        //    Forum forum = AsyncHelper.RunSync(() => forumService.GetForumById(topic != null ? topic.ForumId : (forumId.HasValue ? forumId.Value : 0)));
        //    if (forum != null)
        //    {
        //        model.ForumId = forum.Id;
        //        model.ForumName = forum.Name;
        //        model.ForumSeName = forum.GetSeName();
        //    }

        //    var forumGroup = AsyncHelper.RunSync(() => forumService.GetForumGroupById(forum != null ? forum.ForumGroupId : (forumGroupId.HasValue ? forumGroupId.Value : 0)));
        //    if (forumGroup != null)
        //    {
        //        model.ForumGroupId = forumGroup.Id;
        //        model.ForumGroupName = forumGroup.Name;
        //        model.ForumGroupSeName = forumGroup.GetSeName();
        //    }

        //    return PartialView(model);
        //}

        [Route("subscribe/{page?}")]
        public async Task<IActionResult> UserForumSubscriptions(int? page)
        {
            if (!forumSettings.AllowUsersToManageSubscriptions)
            {
                return RedirectToAction("Index");
                //return RedirectToRoute("UserInfo"); // TODO: Is this meant o be the user's profile page?
            }

            int pageIndex = 0;
            if (page > 0)
            {
                pageIndex = page.Value - 1;
            }

            var customer = WorkContext.CurrentUser;

            var pageSize = forumSettings.ForumSubscriptionsPageSize;

            var subscriptions = await forumService.GetAllSubscriptions(customer.Id, 0, 0, pageIndex, pageSize);

            var model = new UserForumSubscriptionsModel();

            foreach (var subscription in subscriptions)
            {
                int topicId = subscription.TopicId;
                int forumId = subscription.ForumId;
                bool topicSubscription = false;
                string title = string.Empty;
                string slug = string.Empty;

                if (topicId > 0)
                {
                    topicSubscription = true;
                    var topic = await forumService.GetTopicById(topicId);
                    if (topic != null)
                    {
                        title = topic.Subject;
                        slug = topic.GetSeName();
                    }
                }
                else
                {
                    var forum = await forumService.GetForumById(forumId);
                    if (forum != null)
                    {
                        title = forum.Name;
                        slug = forum.GetSeName();
                    }
                }

                model.ForumSubscriptions.Add(new UserForumSubscriptionsModel.ForumSubscriptionModel
                {
                    Id = subscription.Id,
                    ForumTopicId = topicId,
                    ForumId = subscription.ForumId,
                    TopicSubscription = topicSubscription,
                    Title = title,
                    Slug = slug,
                });
            }

            model.PagerModel = new PagerModel
            {
                PageSize = subscriptions.PageSize,
                TotalRecords = subscriptions.ItemCount,
                PageIndex = subscriptions.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "UserForumSubscriptionsPaged",
                //UseRouteLinks = true,
                RouteValues = new ForumSubscriptionsRouteValues { page = pageIndex }
            };

            return View(model);
        }

        [HttpPost]
        [ActionName("UserForumSubscriptions")]
        [Route("subscribe-post")]
        public async Task<IActionResult> UserForumSubscriptionsPOST(FormCollection formCollection)
        {
            foreach (string key in formCollection.Keys)
            {
                string value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("fs", StringComparison.OrdinalIgnoreCase))
                {
                    string id = key.Replace("fs", string.Empty).Trim();
                    int subscriptionId;
                    if (int.TryParse(id, out subscriptionId))
                    {
                        var subscription = await forumService.GetSubscriptionById(subscriptionId);
                        if (subscription != null && subscription.UserId == WorkContext.CurrentUser.Id)
                        {
                            await forumService.DeleteSubscription(subscription);
                        }
                    }
                }
            }

            return RedirectToAction("UserForumSubscriptions");
        }

        [HttpPost]
        [Route("upload-file")]
        public JsonResult UploadFile()
        {
            try
            {
                #region Save File

                var file = Request.Form.Files["Upload"];

                string uploadFileName = Path.Combine(
                    CommonHelper.MapPath("~/Media/Uploads/_Users/" + WorkContext.CurrentUser.Id + "/"),
                    file.FileName);

                string directory = Path.GetDirectoryName(uploadFileName);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fileStream = new FileStream(uploadFileName, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                #endregion Save File

                return Json(new
                {
                    Success = true,
                    Url = string.Format("/Media/Uploads/_Users/{0}/{1}", WorkContext.CurrentUser.Id, file.FileName),
                    FileName = file.FileName
                });
            }
            catch (Exception x)
            {
                return Json(new { Success = false, error = x.GetBaseException().Message });
            }
        }

        #endregion Methods
    }
}