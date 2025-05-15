using Mantle.Infrastructure;
using Mantle.Web;
using Mantle.Web.Navigation;
using Microsoft.Extensions.Localization;

namespace Mantle.Plugins.Widgets.FullCalendar;

public class NavigationProvider : INavigationProvider
{
    public NavigationProvider()
    {
        T = DependoResolver.Instance.Resolve<IStringLocalizer>();
    }

    public IStringLocalizer T { get; set; }

    #region INavigationProvider Members

    public string MenuName => MantleWebConstants.Areas.Admin;

    public void GetNavigation(NavigationBuilder builder) => builder.Add(T[MantleWebLocalizableStrings.Plugins.Title],
        menu => menu.Add(T[LocalizableStrings.FullCalendar], "5", item => item
            .Url("#plugins/widgets/fullcalendar")
            .Icons("fa fa-calendar")
            .Permission(FullCalendarPermissions.ReadCalendar)));

    #endregion INavigationProvider Members
}