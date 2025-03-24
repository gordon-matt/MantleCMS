using Mantle.Web.Navigation;

namespace Mantle.Web.Areas.Admin;

public class NavigationProvider : INavigationProvider
{
    public NavigationProvider()
    {
        T = EngineContext.Current.Resolve<IStringLocalizer>();
    }

    public IStringLocalizer T { get; set; }

    #region INavigationProvider Members

    public string MenuName => MantleWebConstants.Areas.Admin;

    public void GetNavigation(NavigationBuilder builder)
    {
        builder.Add(T[MantleWebLocalizableStrings.Dashboard.Title], "0", BuildHomeMenu);
        builder.Add(T[MantleWebLocalizableStrings.Membership.Title], "1", BuildMembershipMenu);
        builder.Add(T[MantleWebLocalizableStrings.General.Configuration], "3", BuildConfigurationMenu);
        builder.Add(T[MantleWebLocalizableStrings.Maintenance.Title], "4", BuildMaintenanceMenu);
        builder.Add(T[MantleWebLocalizableStrings.Plugins.Title], "99999", BuildPluginsMenu);
    }

    #endregion INavigationProvider Members

    private static void BuildHomeMenu(NavigationItemBuilder builder)
    {
        builder.Permission(StandardPermissions.DashboardAccess);

        builder.Icons("fa fa-dashboard")
            .Url("#");
    }

    private void BuildMembershipMenu(NavigationItemBuilder builder) => builder
        .Url("#membership")
        .Icons("fa fa-users")
        .Permission(MantleWebPermissions.MembershipManage);

    private void BuildConfigurationMenu(NavigationItemBuilder builder)
    {
        builder.Icons("fa fa-cog");

        // Localization
        builder.Add(T[MantleWebLocalizableStrings.Localization.Title], "5", item => item
            .Url("#localization/languages")
            .Icons("fa fa-globe")
            .Permission(MantleWebPermissions.LanguagesRead));

        //// Indexing
        //builder.Add(T[MantleWebLocalizableStrings.Indexing.Title], "5", item => item
        //    .Url("#indexing")
        //    .Icons("fa fa-search")
        //    .Permission(StandardPermissions.FullAccess));

        // Plugins
        builder.Add(T[MantleWebLocalizableStrings.Plugins.Title], "5", item => item
            .Url("#plugins")
            .Icons("fa fa-puzzle-piece")
            .Permission(StandardPermissions.FullAccess));

        // Settings
        builder.Add(T[MantleWebLocalizableStrings.General.Settings], "5", item => item
            .Url("#configuration/settings")
            .Icons("fa fa-cog")
            .Permission(MantleWebPermissions.SettingsRead));

        // Tenants
        builder.Add(T[MantleWebLocalizableStrings.Tenants.Title], "5", item => item
            .Url("#tenants")
            .Icons("fa fa-building-o")
            .Permission(StandardPermissions.FullAccess));

        // Themes
        builder.Add(T[MantleWebLocalizableStrings.General.Themes], "5", item => item
            .Url("#configuration/themes")
            .Icons("fa fa-tint")
            .Permission(MantleWebPermissions.ThemesRead));
    }

    private void BuildMaintenanceMenu(NavigationItemBuilder builder)
    {
        builder.Icons("fa fa-wrench");

        // Log
        builder.Add(T[MantleWebLocalizableStrings.Log.Title], "5", item => item
            .Url("#log")
            .Icons("fa fa-warning")
            .Permission(MantleWebPermissions.LogRead));

        // Scheduled Tasks
        builder.Add(T[MantleWebLocalizableStrings.ScheduledTasks.Title], "5", item => item
            .Url("#scheduled-tasks")
            .Icons("fa fa-clock-o")
            .Permission(MantleWebPermissions.ScheduledTasksRead));
    }

    private void BuildPluginsMenu(NavigationItemBuilder builder) => builder.Icons("fa fa-puzzle-piece");
}