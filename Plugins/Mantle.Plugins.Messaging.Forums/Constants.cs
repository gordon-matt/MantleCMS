namespace Mantle.Plugins.Messaging.Forums;

public static class Constants
{
    public const string PluginSystemName = "Mantle.Plugins.Messaging.Forums";
    public const string RouteArea = "Plugins/Messaging/Forums";

    public static class Roles
    {
        public const string ForumModerators = "Forum Moderators";
    }

    public static class Tables
    {
        public const string Groups = "Forums_Groups";
        public const string Forums = "Forums_Forums";
        public const string Posts = "Forums_Posts";
        public const string Subscriptions = "Forums_Subscriptions";
        public const string Topics = "Forums_Topics";
        public const string PrivateMessages = "Forums_PrivateMessages";
    }
}