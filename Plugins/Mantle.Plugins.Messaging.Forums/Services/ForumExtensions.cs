namespace Mantle.Plugins.Messaging.Forums.Services;

public static class ForumExtensions
{
    extension(Forum forum)
    {
        public async Task<ForumTopic> GetLastTopicAsync(IForumService forumService) => forum == null
            ? throw new ArgumentNullException(nameof(forum))
            : await forumService.GetTopicById(forum.LastTopicId);

        public async Task<ForumPost> GetLastPostAsync(IForumService forumService) => forum == null
            ? throw new ArgumentNullException(nameof(forum))
            : await forumService.GetPostById(forum.LastPostId);

        public async Task<MantleUser> GetLastPostCustomerAsync(IMembershipService membershipService) => forum == null
            ? throw new ArgumentNullException(nameof(forum))
            : await membershipService.GetUserById(forum.LastPostUserId);
    }

    extension(ForumPost forumPost)
    {
        public string FormatPostText()
        {
            string text = forumPost.Text;

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var editor = DependoResolver.Instance.Resolve<ForumSettings>().ForumEditor;
            switch (editor)
            {
                case EditorType.SimpleTextBox:
                    text = Html.HtmlHelper.FormatText(text, false, true, false, false, false, false);
                    break;

                case EditorType.BBCodeEditor:
                    text = Html.HtmlHelper.FormatText(text, false, true, false, true, false, false);
                    break;

                default:
                    break;
            }

            return text;
        }
    }

    extension(ForumTopic forumTopic)
    {
        public string StripTopicSubject()
        {
            string subject = forumTopic.Subject;
            if (string.IsNullOrEmpty(subject))
            {
                return subject;
            }

            int strippedTopicMaxLength = DependoResolver.Instance.Resolve<ForumSettings>().StrippedTopicMaxLength;
            if (strippedTopicMaxLength > 0)
            {
                if (subject.Length > strippedTopicMaxLength)
                {
                    int index = subject.IndexOf(" ", strippedTopicMaxLength);
                    if (index > 0)
                    {
                        subject = subject[..index];
                        subject += "...";
                    }
                }
            }

            return subject;
        }

        public async Task<ForumPost> GetFirstPostAsync(IForumService forumService)
        {
            ArgumentNullException.ThrowIfNull(forumTopic);
            var forumPosts = await forumService.GetAllPosts(forumTopic.Id, null, string.Empty, 0, 1);
            return forumPosts.Count > 0 ? forumPosts.First() : null;
        }

        public async Task<ForumPost> GetLastPostAsync(IForumService forumService) => forumTopic == null
            ? throw new ArgumentNullException(nameof(forumTopic))
            : await forumService.GetPostById(forumTopic.LastPostId);

        public async Task<MantleUser> GetLastPostCustomerAsync(IMembershipService membershipService) => forumTopic == null
            ? throw new ArgumentNullException(nameof(forumTopic))
            : await membershipService.GetUserById(forumTopic.LastPostUserId);
    }

    extension(PrivateMessage pm)
    {
        public string FormatPrivateMessageText()
        {
            string text = pm.Text;

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            text = Html.HtmlHelper.FormatText(text, false, true, false, true, false, false);

            return text;
        }
    }

    extension(string text)
    {
        public string FormatForumSignatureText()
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            text = Html.HtmlHelper.FormatText(text, false, true, false, false, false, false);
            return text;
        }
    }
}