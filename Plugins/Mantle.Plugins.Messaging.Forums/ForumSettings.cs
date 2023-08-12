namespace Mantle.Plugins.Messaging.Forums;

public class ForumSettings : BaseResourceSettings
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

    public override string Name => "Forum Settings";

    public override string EditorTemplatePath => "/Plugins/Messaging.Forums/Views/Shared/EditorTemplates/ForumSettings.cshtml";

    #endregion ISettings Members

    #region IResourceSettings Members

    public override ICollection<RequiredResourceCollection> Resources { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public override ICollection<RequiredResourceCollection> DefaultResources => new List<RequiredResourceCollection>
    {
        new RequiredResourceCollection
        {
            Name = "Bootstrap-FileInput",
            Resources = new List<RequiredResource>
            {
                new RequiredResource { Type = ResourceType.Script, Order = 0, Path = "https://cdn.jsdelivr.net/npm/bootstrap-fileinput@5.5.2/js/fileinput.min.js" },
                new RequiredResource { Type = ResourceType.Stylesheet, Order = 0, Path = "https://cdn.jsdelivr.net/npm/bootstrap-fileinput@5.5.2/css/fileinput.min.css" }
            }
        }
    };

    #endregion IResourceSettings Members

    [LocalizedDisplayName(LocalizableStrings.Settings.ForumsEnabled)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ForumsEnabled)]
    [SettingsProperty(true)]
    public bool ForumsEnabled { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.RelativeDateTimeFormattingEnabled)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.RelativeDateTimeFormattingEnabled)]
    [SettingsProperty(true)]
    public bool RelativeDateTimeFormattingEnabled { get; set; }

    #region Permissions

    [LocalizedDisplayName(LocalizableStrings.Settings.AllowUsersToEditPosts)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.AllowUsersToEditPosts)]
    [SettingsProperty(true)]
    public bool AllowUsersToEditPosts { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.AllowUsersToManageSubscriptions)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.AllowUsersToManageSubscriptions)]
    [SettingsProperty(true)]
    public bool AllowUsersToManageSubscriptions { get; set; }

    //[LocalizedDisplayName(LocalizableStrings.Settings.AllowGuestsToCreatePosts)]
    //[LocalizedHelpText(LocalizableStrings.Settings.HelpText.AllowGuestsToCreatePosts)]
    //public bool AllowGuestsToCreatePosts { get; set; }

    //[LocalizedDisplayName(LocalizableStrings.Settings.AllowGuestsToCreateTopics)]
    //[LocalizedHelpText(LocalizableStrings.Settings.HelpText.AllowGuestsToCreateTopics)]
    //public bool AllowGuestsToCreateTopics { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.AllowUsersToDeletePosts)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.AllowUsersToDeletePosts)]
    [SettingsProperty(true)]
    public bool AllowUsersToDeletePosts { get; set; }

    #endregion Permissions

    #region Text Lengths

    [LocalizedDisplayName(LocalizableStrings.Settings.TopicSubjectMaxLength)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.TopicSubjectMaxLength)]
    [SettingsProperty(255)]
    public int TopicSubjectMaxLength { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.StrippedTopicMaxLength)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.StrippedTopicMaxLength)]
    [SettingsProperty(255)]
    public int StrippedTopicMaxLength { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.PostMaxLength)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.PostMaxLength)]
    [SettingsProperty(2048)]
    public int PostMaxLength { get; set; }

    #endregion Text Lengths

    #region Page Sizes

    [LocalizedDisplayName(LocalizableStrings.Settings.TopicsPageSize)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.TopicsPageSize)]
    [SettingsProperty(10)]
    public int TopicsPageSize { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.PostsPageSize)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.PostsPageSize)]
    [SettingsProperty(10)]
    public int PostsPageSize { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.SearchResultsPageSize)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.SearchResultsPageSize)]
    [SettingsProperty(10)]
    public int SearchResultsPageSize { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.ActiveDiscussionsPageSize)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ActiveDiscussionsPageSize)]
    [SettingsProperty(10)]
    public int ActiveDiscussionsPageSize { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.LatestUserPostsPageSize)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.LatestUserPostsPageSize)]
    [SettingsProperty(10)]
    public int LatestUserPostsPageSize { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.ForumSubscriptionsPageSize)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ForumSubscriptionsPageSize)]
    [SettingsProperty(10)]
    public int ForumSubscriptionsPageSize { get; set; }

    #endregion Page Sizes

    [LocalizedDisplayName(LocalizableStrings.Settings.ShowUsersPostCount)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ShowUsersPostCount)]
    [SettingsProperty(true)]
    public bool ShowUsersPostCount { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.ForumEditor)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ForumEditor)]
    [SettingsProperty(EditorType.BBCodeEditor)]
    public EditorType ForumEditor { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.SignaturesEnabled)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.SignaturesEnabled)]
    [SettingsProperty(true)]
    public bool SignaturesEnabled { get; set; }

    #region Private Messages

    [LocalizedDisplayName(LocalizableStrings.Settings.AllowPrivateMessages)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.AllowPrivateMessages)]
    [SettingsProperty]
    public bool AllowPrivateMessages { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.ShowAlertForPM)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ShowAlertForPM)]
    [SettingsProperty(true)]
    public bool ShowAlertForPM { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.PrivateMessagesPageSize)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.PrivateMessagesPageSize)]
    [SettingsProperty(10)]
    public int PrivateMessagesPageSize { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.NotifyAboutPrivateMessages)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.NotifyAboutPrivateMessages)]
    [SettingsProperty(true)]
    public bool NotifyAboutPrivateMessages { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.PMSubjectMaxLength)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.PMSubjectMaxLength)]
    [SettingsProperty]
    public int PMSubjectMaxLength { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.PMTextMaxLength)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.PMTextMaxLength)]
    [SettingsProperty]
    public int PMTextMaxLength { get; set; }

    #endregion Private Messages

    [LocalizedDisplayName(LocalizableStrings.Settings.HomePageActiveDiscussionsTopicCount)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.HomePageActiveDiscussionsTopicCount)]
    [SettingsProperty]
    public int HomePageActiveDiscussionsTopicCount { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.ForumSearchTermMinimumLength)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ForumSearchTermMinimumLength)]
    [SettingsProperty(2)]
    public int ForumSearchTermMinimumLength { get; set; }

    #region RSS Feeds

    [LocalizedDisplayName(LocalizableStrings.Settings.ForumFeedsEnabled)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ForumFeedsEnabled)]
    [SettingsProperty(true)]
    public bool ForumFeedsEnabled { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.ForumFeedCount)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ForumFeedCount)]
    [SettingsProperty(10)]
    public int ForumFeedCount { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.ActiveDiscussionsFeedEnabled)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ActiveDiscussionsFeedEnabled)]
    [SettingsProperty(true)]
    public bool ActiveDiscussionsFeedEnabled { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.ActiveDiscussionsFeedCount)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.ActiveDiscussionsFeedCount)]
    [SettingsProperty(10)]
    public int ActiveDiscussionsFeedCount { get; set; }

    #endregion RSS Feeds

    [LocalizedDisplayName(LocalizableStrings.Settings.PageTitle)]
    [SettingsProperty]
    public string PageTitle { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.ShowOnMenus)]
    [SettingsProperty(true)]
    public bool ShowOnMenus { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.MenuPosition)]
    [SettingsProperty]
    public byte MenuPosition { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.LayoutPathOverride)]
    [LocalizedHelpText(LocalizableStrings.Settings.HelpText.LayoutPathOverride)]
    [SettingsProperty]
    public string LayoutPathOverride { get; set; }
}