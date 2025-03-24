using System.Globalization;
using Mantle.Web.Navigation;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace Mantle.Web.Mvc.Razor;

public abstract class MantleRazorPage<TModel> : RazorPage<TModel>
{
    public IEnumerable<MenuItem> GetMenu(string menuName) => EngineContext.Current.Resolve<INavigationManager>().BuildMenu(menuName);

    [RazorInject]
    public SiteSettings SiteSettings { get; set; }

    [RazorInject]
    public IStringLocalizer T { get; set; }

    [RazorInject]
    public IWorkContext WorkContext { get; set; }

    public bool IsRightToLeft => CultureInfo.CurrentCulture.TextInfo.IsRightToLeft;

    public bool CheckPermission(Permission permission)
    {
        var authorizationService = EngineContext.Current.Resolve<IMantleAuthorizationService>();
        return authorizationService.TryCheckAccess(permission, WorkContext.CurrentUser);
    }
}

public abstract class MantleRazorPage : MantleRazorPage<dynamic>
{
}