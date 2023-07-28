using Mantle.Web;
using Mantle.Web.Navigation;
using Microsoft.Extensions.Localization;

namespace MantleCMS.Areas.Admin;

public class AdminNavigationProvider : INavigationProvider
{
    public AdminNavigationProvider()
    {
        T = EngineContext.Current.Resolve<IStringLocalizer>();
    }

    public IStringLocalizer T { get; set; }

    #region INavigationProvider Members

    public string MenuName
    {
        get { return MantleWebConstants.Areas.Admin; }
    }

    public void GetNavigation(NavigationBuilder builder)
    {
        builder.Add(T[MantleWebLocalizableStrings.Dashboard.Title], "0", BuildDashboardMenu);
    }

    private static void BuildDashboardMenu(NavigationItemBuilder builder)
    {
        builder.Icons("fa fa-dashboard");
    }

    #endregion INavigationProvider Members
}