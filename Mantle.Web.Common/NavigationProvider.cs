using Mantle.Web.Navigation;

namespace Mantle.Web.Common;

public class NavigationProvider : INavigationProvider
{
    public NavigationProvider()
    {
        T = EngineContext.Current.Resolve<IStringLocalizer>();
    }

    public IStringLocalizer T { get; set; }

    public string MenuName => MantleWebConstants.Areas.Admin;

    public void GetNavigation(NavigationBuilder builder) => builder.Add(T[MantleWebLocalizableStrings.General.Configuration],
        menu => menu.Add(T[LocalizableStrings.Regions.Title], "5", item => item
            .Url("#regions")
            //.Action("Index", "Region", new { area = Constants.Areas.Regions })
            .Icons("fa fa-globe")
            .Permission(Permissions.RegionsRead)));
}