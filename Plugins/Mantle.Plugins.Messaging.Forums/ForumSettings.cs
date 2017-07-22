using Mantle.ComponentModel;
using Mantle.Plugins.Messaging.Forums.Data.Domain;
using Mantle.Web.Configuration;

namespace Mantle.Plugins.Messaging.Forums
{
    public class ForumSettings : ISettings
    {
        public ForumSettings()
        {
            ForumsEnabled = true;
            RelativeDateTimeFormattingEnabled = true;

            AllowUsersToEditPosts = true;
            AllowUsersToManageSubscriptions = true;
            AllowUsersToDeletePosts = true;

            TopicSubjectMaxLength = 255;
            StrippedTopicMaxLength = 255;
            PostMaxLength = 2048;

            TopicsPageSize = 10;
            PostsPageSize = 10;
            SearchResultsPageSize = 10;
            ActiveDiscussionsPageSize = 10;
            LatestUserPostsPageSize = 10;
            ForumSubscriptionsPageSize = 10;

            ShowUsersPostCount = true;
            ForumEditor = EditorType.BBCodeEditor;
            SignaturesEnabled = true;

            AllowPrivateMessages = false;
            ShowAlertForPM = true;
            PrivateMessagesPageSize = 10;
            NotifyAboutPrivateMessages = true;
            PMSubjectMaxLength = 128;
            PMTextMaxLength = 1024;

            HomePageActiveDiscussionsTopicCount = 10;
            ForumSearchTermMinimumLength = 2;

            ForumFeedsEnabled = true;
            ForumFeedCount = 10;
            ActiveDiscussionsFeedEnabled = true;
            ActiveDiscussionsFeedCount = 10;

            ShowOnMenus = true;
        }

        #region ISettings Members

        public string Name
        {
            get { return "Forum Settings"; }
        }

        public bool IsTenantRestricted
        {
            get { return false; }
        }

        public string EditorTemplatePath
        {
            get { return "/Plugins/Messaging.Forums/Views/Shared/EditorTemplates/ForumSettings.cshtml"; }
        }

        #endregion ISettings Members

        [LocalizedDisplayName(LocalizableStrings.Settings.ForumsEnabled)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ForumsEnabled)]
        public bool ForumsEnabled { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.RelativeDateTimeFormattingEnabled)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.RelativeDateTimeFormattingEnabled)]
        public bool RelativeDateTimeFormattingEnabled { get; set; }

        #region Permissions

        [LocalizedDisplayName(LocalizableStrings.Settings.AllowUsersToEditPosts)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.AllowUsersToEditPosts)]
        public bool AllowUsersToEditPosts { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.AllowUsersToManageSubscriptions)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.AllowUsersToManageSubscriptions)]
        public bool AllowUsersToManageSubscriptions { get; set; }

        //[LocalizedDisplayName(LocalizableStrings.Settings.AllowGuestsToCreatePosts)]
        //[LocalizedHelpText(LocalizableStrings.Settings.HelpText.AllowGuestsToCreatePosts)]
        //public bool AllowGuestsToCreatePosts { get; set; }

        //[LocalizedDisplayName(LocalizableStrings.Settings.AllowGuestsToCreateTopics)]
        //[LocalizedHelpText(LocalizableStrings.Settings.HelpText.AllowGuestsToCreateTopics)]
        //public bool AllowGuestsToCreateTopics { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.AllowUsersToDeletePosts)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.AllowUsersToDeletePosts)]
        public bool AllowUsersToDeletePosts { get; set; }

        #endregion

        #region Text Lengths

        [LocalizedDisplayName(LocalizableStrings.Settings.TopicSubjectMaxLength)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.TopicSubjectMaxLength)]
        public int TopicSubjectMaxLength { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.StrippedTopicMaxLength)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.StrippedTopicMaxLength)]
        public int StrippedTopicMaxLength { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.PostMaxLength)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.PostMaxLength)]
        public int PostMaxLength { get; set; }

        #endregion

        #region Page Sizes

        [LocalizedDisplayName(LocalizableStrings.Settings.TopicsPageSize)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.TopicsPageSize)]
        public int TopicsPageSize { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.PostsPageSize)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.PostsPageSize)]
        public int PostsPageSize { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.SearchResultsPageSize)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.SearchResultsPageSize)]
        public int SearchResultsPageSize { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.ActiveDiscussionsPageSize)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ActiveDiscussionsPageSize)]
        public int ActiveDiscussionsPageSize { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.LatestUserPostsPageSize)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.LatestUserPostsPageSize)]
        public int LatestUserPostsPageSize { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.ForumSubscriptionsPageSize)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ForumSubscriptionsPageSize)]
        public int ForumSubscriptionsPageSize { get; set; }

        #endregion

        [LocalizedDisplayName(LocalizableStrings.Settings.ShowUsersPostCount)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ShowUsersPostCount)]
        public bool ShowUsersPostCount { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.ForumEditor)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ForumEditor)]
        public EditorType ForumEditor { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.SignaturesEnabled)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.SignaturesEnabled)]
        public bool SignaturesEnabled { get; set; }

        #region Private Messages

        [LocalizedDisplayName(LocalizableStrings.Settings.AllowPrivateMessages)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.AllowPrivateMessages)]
        public bool AllowPrivateMessages { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.ShowAlertForPM)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ShowAlertForPM)]
        public bool ShowAlertForPM { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.PrivateMessagesPageSize)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.PrivateMessagesPageSize)]
        public int PrivateMessagesPageSize { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.NotifyAboutPrivateMessages)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.NotifyAboutPrivateMessages)]
        public bool NotifyAboutPrivateMessages { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.PMSubjectMaxLength)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.PMSubjectMaxLength)]
        public int PMSubjectMaxLength { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.PMTextMaxLength)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.PMTextMaxLength)]
        public int PMTextMaxLength { get; set; }

        #endregion

        [LocalizedDisplayName(LocalizableStrings.Settings.HomePageActiveDiscussionsTopicCount)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.HomePageActiveDiscussionsTopicCount)]
        public int HomePageActiveDiscussionsTopicCount { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.ForumSearchTermMinimumLength)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ForumSearchTermMinimumLength)]
        public int ForumSearchTermMinimumLength { get; set; }

        #region RSS Feeds

        [LocalizedDisplayName(LocalizableStrings.Settings.ForumFeedsEnabled)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ForumFeedsEnabled)]
        public bool ForumFeedsEnabled { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.ForumFeedCount)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ForumFeedCount)]
        public int ForumFeedCount { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.ActiveDiscussionsFeedEnabled)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ActiveDiscussionsFeedEnabled)]
        public bool ActiveDiscussionsFeedEnabled { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.ActiveDiscussionsFeedCount)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ActiveDiscussionsFeedCount)]
        public int ActiveDiscussionsFeedCount { get; set; }

        #endregion

        [LocalizedDisplayName(LocalizableStrings.Settings.PageTitle)]
        public string PageTitle { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.ShowOnMenus)]
        public bool ShowOnMenus { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.MenuPosition)]
        public byte MenuPosition { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.LayoutPathOverride)]
        [LocalizedHelpText(LocalizableStrings.Settings.HelpText.LayoutPathOverride)]
        public string LayoutPathOverride { get; set; }
    }
}