using Mantle.Web.Navigation;

namespace Mantle.Plugins.Messaging.Forums;

public class NavigationProvider : INavigationProvider
{
    public NavigationProvider()
    {
        T = EngineContext.Current.Resolve<IStringLocalizer>();
    }

    public IStringLocalizer T { get; set; }

    public string MenuName => MantleWebConstants.Areas.Admin;

    public void GetNavigation(NavigationBuilder builder)
    {
        builder.Add(T[MantleWebLocalizableStrings.Plugins.Title],
            menu => menu.Add(T[LocalizableStrings.Forums], "5", item => item
                .Url("#plugins/messaging/forums")
                .Icons("fa fa-comments")
                .Permission(ForumPermissions.ReadForums)));
    }
}