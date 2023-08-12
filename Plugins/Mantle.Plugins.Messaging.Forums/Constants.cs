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
        public const string Groups = "Mantle_Plugins_Forums_Groups";
        public const string Forums = "Mantle_Plugins_Forums_Forums";
        public const string Posts = "Mantle_Plugins_Forums_Posts";
        public const string Subscriptions = "Mantle_Plugins_Forums_Subscriptions";
        public const string Topics = "Mantle_Plugins_Forums_Topics";
        public const string PrivateMessages = "Mantle_Plugins_Forums_PrivateMessages";
    }
}