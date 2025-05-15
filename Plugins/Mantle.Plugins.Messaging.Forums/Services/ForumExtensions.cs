namespace Mantle.Plugins.Messaging.Forums.Services;

public static class ForumExtensions
{
    public static string FormatPostText(this ForumPost forumPost)
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

    public static string StripTopicSubject(this ForumTopic forumTopic)
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

    public static string FormatForumSignatureText(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        text = Html.HtmlHelper.FormatText(text, false, true, false, false, false, false);
        return text;
    }

    public static string FormatPrivateMessageText(this PrivateMessage pm)
    {
        string text = pm.Text;

        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        text = Html.HtmlHelper.FormatText(text, false, true, false, true, false, false);

        return text;
    }

    public static ForumTopic GetLastTopic(this Forum forum, IForumService forumService) => forum == null
        ? throw new ArgumentNullException(nameof(forum))
        : AsyncHelper.RunSync(() => forumService.GetTopicById(forum.LastTopicId));

    public static ForumPost GetLastPost(this Forum forum, IForumService forumService) => forum == null
        ? throw new ArgumentNullException(nameof(forum))
        : AsyncHelper.RunSync(() => forumService.GetPostById(forum.LastPostId));

    public static MantleUser GetLastPostCustomer(this Forum forum, IMembershipService membershipService) => forum == null
        ? throw new ArgumentNullException(nameof(forum))
        : AsyncHelper.RunSync(() => membershipService.GetUserById(forum.LastPostUserId));

    public static ForumPost GetFirstPost(this ForumTopic forumTopic, IForumService forumService)
    {
        ArgumentNullException.ThrowIfNull(forumTopic);

        var forumPosts = AsyncHelper.RunSync(() => forumService.GetAllPosts(forumTopic.Id, null, string.Empty, 0, 1));
        return forumPosts.Count > 0 ? forumPosts.First() : null;
    }

    public static ForumPost GetLastPost(this ForumTopic forumTopic, IForumService forumService) => forumTopic == null
        ? throw new ArgumentNullException(nameof(forumTopic))
        : AsyncHelper.RunSync(() => forumService.GetPostById(forumTopic.LastPostId));

    public static MantleUser GetLastPostCustomer(this ForumTopic forumTopic, IMembershipService membershipService) => forumTopic == null
        ? throw new ArgumentNullException(nameof(forumTopic))
        : AsyncHelper.RunSync(() => membershipService.GetUserById(forumTopic.LastPostUserId));
}