using Mantle.Infrastructure;
using Mantle.Web;
using Mantle.Web.Navigation;
using Microsoft.Extensions.Localization;

namespace Mantle.Plugins.Messaging.Forums
{
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
                    .Icons("kore-icon kore-icon-forums")
                    .Permission(ForumPermissions.ReadForums)));
        }
    }
}