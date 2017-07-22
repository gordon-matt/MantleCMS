using System;
using Mantle.Infrastructure;
using Mantle.Plugins.Messaging.Forums.Data.Domain;
using Mantle.Security.Membership;
using Mantle.Threading;
using Mantle.Web.Html;

namespace Mantle.Plugins.Messaging.Forums.Services
{
    public static class ForumExtensions
    {
        public static string FormatPostText(this ForumPost forumPost)
        {
            string text = forumPost.Text;

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var editor = EngineContext.Current.Resolve<ForumSettings>().ForumEditor;
            switch (editor)
            {
                case EditorType.SimpleTextBox:
                    {
                        text = HtmlHelper.FormatText(text, false, true, false, false, false, false);
                    }
                    break;

                case EditorType.BBCodeEditor:
                    {
                        text = HtmlHelper.FormatText(text, false, true, false, true, false, false);
                    }
                    break;

                default:
                    break;
            }

            return text;
        }

        public static string StripTopicSubject(this ForumTopic forumTopic)
        {
            string subject = forumTopic.Subject;
            if (string.IsNullOrEmpty(subject))
            {
                return subject;
            }

            int strippedTopicMaxLength = EngineContext.Current.Resolve<ForumSettings>().StrippedTopicMaxLength;
            if (strippedTopicMaxLength > 0)
            {
                if (subject.Length > strippedTopicMaxLength)
                {
                    int index = subject.IndexOf(" ", strippedTopicMaxLength);
                    if (index > 0)
                    {
                        subject = subject.Substring(0, index);
                        subject += "...";
                    }
                }
            }

            return subject;
        }

        public static string FormatForumSignatureText(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);
            return text;
        }

        public static string FormatPrivateMessageText(this PrivateMessage pm)
        {
            string text = pm.Text;

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            text = HtmlHelper.FormatText(text, false, true, false, true, false, false);

            return text;
        }

        public static ForumTopic GetLastTopic(this Forum forum, IForumService forumService)
        {
            if (forum == null)
            {
                throw new ArgumentNullException("forum");
            }

            return AsyncHelper.RunSync(() => forumService.GetTopicById(forum.LastTopicId));
        }

        public static ForumPost GetLastPost(this Forum forum, IForumService forumService)
        {
            if (forum == null)
            {
                throw new ArgumentNullException("forum");
            }

            return AsyncHelper.RunSync(() => forumService.GetPostById(forum.LastPostId));
        }

        public static MantleUser GetLastPostCustomer(this Forum forum, IMembershipService membershipService)
        {
            if (forum == null)
            {
                throw new ArgumentNullException("forum");
            }

            return AsyncHelper.RunSync(() => membershipService.GetUserById(forum.LastPostUserId));
        }

        public static ForumPost GetFirstPost(this ForumTopic forumTopic, IForumService forumService)
        {
            if (forumTopic == null)
            {
                throw new ArgumentNullException("forumTopic");
            }

            var forumPosts = AsyncHelper.RunSync(() => forumService.GetAllPosts(forumTopic.Id, null, string.Empty, 0, 1));
            if (forumPosts.Count > 0)
            {
                return forumPosts[0];
            }

            return null;
        }

        public static ForumPost GetLastPost(this ForumTopic forumTopic, IForumService forumService)
        {
            if (forumTopic == null)
            {
                throw new ArgumentNullException("forumTopic");
            }

            return AsyncHelper.RunSync(() => forumService.GetPostById(forumTopic.LastPostId));
        }

        public static MantleUser GetLastPostCustomer(this ForumTopic forumTopic, IMembershipService membershipService)
        {
            if (forumTopic == null)
            {
                throw new ArgumentNullException("forumTopic");
            }

            return AsyncHelper.RunSync(() => membershipService.GetUserById(forumTopic.LastPostUserId));
        }
    }
}