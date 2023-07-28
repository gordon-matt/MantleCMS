namespace MantleCMS;

public static class Constants
{
    public static class Roles
    {
        public const string Administrators = "Administrators";
    }

    public static class Tables
    {
        // Membership tables (TODO: Consider moving to framework)
        public const string Permissions = "Permissions";

        public const string RolePermissions = "RolePermissions";

        public const string UserProfiles = "UserProfiles";
    }
}