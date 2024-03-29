﻿namespace Mantle.Web.Areas.Admin;

public class MantleWebPermissions : IPermissionProvider
{
    // Localization
    public static readonly Permission LanguagesRead = new() { Name = "Languages_Read", Category = "Localization", Description = "Mantle - Languages: Read" };

    public static readonly Permission LanguagesWrite = new() { Name = "Languages_Write", Category = "Localization", Description = "Mantle - Languages: Write" };
    public static readonly Permission LocalizableStringsRead = new() { Name = "LocalizableStrings_Read", Category = "Localization", Description = "Mantle - Localizable Strings: Read" };
    public static readonly Permission LocalizableStringsWrite = new() { Name = "LocalizableStrings_Write", Category = "Localization", Description = "Mantle - Localizable Strings: Write" };

    // Log
    public static readonly Permission LogRead = new() { Name = "Log_Read", Category = "Log", Description = "Mantle - Log: Read" };

    // Plugins
    public static readonly Permission PluginsManage = new() { Name = "Plugins_Manage", Category = "System", Description = "Mantle - Plugins: Manage" };

    // Scheduled Tasks
    public static readonly Permission ScheduledTasksRead = new() { Name = "Scheduled_Tasks_Read", Category = "System", Description = "Mantle - Scheduled Tasks: Read" };

    public static readonly Permission ScheduledTasksWrite = new() { Name = "Scheduled_Tasks_Write", Category = "System", Description = "Mantle - Scheduled Tasks: Write" };

    // Settings
    public static readonly Permission SettingsRead = new() { Name = "Settings_Read", Category = "Configuration", Description = "Mantle - Settings: Read" };

    public static readonly Permission SettingsWrite = new() { Name = "Settings_Write", Category = "Configuration", Description = "Mantle - Settings: Write" };

    // Themes
    public static readonly Permission ThemesRead = new() { Name = "Themes_Read", Category = "Configuration", Description = "Mantle - Themes: Read" };

    public static readonly Permission ThemesWrite = new() { Name = "Themes_Write", Category = "Configuration", Description = "Mantle - Themes: Write" };

    // Membership
    public static readonly Permission MembershipManage = new() { Name = "Membership_Manage", Category = "Membership", Description = "Mantle - Membership: Manage" };

    public static readonly Permission MembershipPermissionsRead = new() { Name = "Membership_Permissions_Read", Category = "Membership", Description = "Mantle - Membership: Read Permissions" };
    public static readonly Permission MembershipPermissionsWrite = new() { Name = "Membership_Permissions_Write", Category = "Membership", Description = "Mantle - Membership: Write Permissions" };
    public static readonly Permission MembershipRolesRead = new() { Name = "Membership_Roles_Read", Category = "Membership", Description = "Mantle - Membership: Read Roles" };
    public static readonly Permission MembershipRolesWrite = new() { Name = "Membership_Roles_Write", Category = "Membership", Description = "Mantle - Membership: Write Roles" };
    public static readonly Permission MembershipUsersRead = new() { Name = "Membership_Users_Read", Category = "Membership", Description = "Mantle - Membership: Read Users" };
    public static readonly Permission MembershipUsersWrite = new() { Name = "Membership_Users_Write", Category = "Membership", Description = "Mantle - Membership: Read Users" };

    public IEnumerable<Permission> GetPermissions()
    {
        yield return LanguagesRead;
        yield return LanguagesWrite;
        yield return LocalizableStringsRead;
        yield return LocalizableStringsWrite;

        yield return LogRead;

        yield return PluginsManage;

        yield return ScheduledTasksRead;
        yield return ScheduledTasksWrite;

        yield return SettingsRead;
        yield return SettingsWrite;

        yield return ThemesRead;
        yield return ThemesWrite;

        yield return MembershipManage;
        yield return MembershipPermissionsRead;
        yield return MembershipPermissionsWrite;
        yield return MembershipRolesRead;
        yield return MembershipRolesWrite;
        yield return MembershipUsersRead;
        yield return MembershipUsersWrite;
    }
}