using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Mantle.Web.Navigation;

public class NavigationManager : INavigationManager
{
    private readonly IMantleAuthorizationService authorizationService;
    private readonly IEnumerable<INavigationProvider> providers;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILogger logger;
    private readonly IUrlHelper urlHelper;
    private readonly IWebHelper webHelper;
    private readonly IWorkContext workContext;

    public NavigationManager(
        IActionContextAccessor actionContextAccessor,
        IEnumerable<INavigationProvider> providers,
        IHttpContextAccessor httpContextAccessor,
        ILoggerFactory loggerFactory,
        IUrlHelperFactory urlHelperFactory,
        IWebHelper webHelper,
        IWorkContext workContext,
        IMantleAuthorizationService authorizationService = null)
    {
        this.authorizationService = authorizationService;
        this.httpContextAccessor = httpContextAccessor;
        this.logger = loggerFactory.CreateLogger<NavigationManager>();
        this.providers = providers;
        this.urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        this.webHelper = webHelper;
        this.workContext = workContext;
    }

    #region INavigationManager Members

    public IEnumerable<MenuItem> BuildMenu(string menuName)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var sources = GetSources(menuName);
        string url = UriHelper.BuildAbsolute(httpContext.Request.Scheme, httpContext.Request.Host);
        return FinishMenu(Reduce(Merge(sources), workContext).ToArray(), url);
    }

    private IEnumerable<IEnumerable<MenuItem>> GetSources(string menuName)
    {
        foreach (var provider in providers.Where(x => x.MenuName == menuName))
        {
            var builder = new NavigationBuilder();
            IEnumerable<MenuItem> items = null;
            try
            {
                provider.GetNavigation(builder);
                items = builder.Build();
            }
            catch (Exception x)
            {
                logger.LogError(
                    new EventId(), x, "Unexpected error while querying a navigation provider. It was ignored. The menu provided by the provider may not be complete.");
            }

            if (items != null)
            {
                yield return items;
            }
        }
    }

    private IEnumerable<MenuItem> FinishMenu(IEnumerable<MenuItem> menuItems, string currentUrl)
    {
        foreach (var menuItem in menuItems)
        {
            menuItem.Href = GetUrl(menuItem.Url, menuItem.RouteValues);

            if (currentUrl.Equals(menuItem.Href))
            {
                menuItem.Selected = true;
            }

            menuItem.Items = FinishMenu(menuItem.Items.ToArray(), currentUrl);
        }

        return menuItems;
    }

    private IEnumerable<MenuItem> Reduce(IEnumerable<MenuItem> items, IWorkContext workContext)
    {
        foreach (var item in items)
        {
            if (authorizationService == null || !item.Permissions.Any() || item.Permissions.Any(x => authorizationService.TryCheckAccess(x, workContext.CurrentUser)))
            {
                yield return new MenuItem
                {
                    Items = Reduce(item.Items, workContext),
                    Permissions = item.Permissions,
                    Position = item.Position,
                    RouteValues = item.RouteValues,
                    Text = item.Text,
                    CssClass = item.CssClass,
                    Icons = item.Icons,
                    Url = item.Url,
                    Href = item.Href
                };
            }
        }
    }

    private static IEnumerable<MenuItem> Merge(IEnumerable<IEnumerable<MenuItem>> sources)
    {
        var comparer = new MenuItemComparer();
        var orderer = new FlatPositionComparer();

        return sources.SelectMany(x => x).ToArray()
            // group same menus
            .GroupBy(key => key, (key, items) => Join(items.ToList()), comparer)
            // group same position
            .GroupBy(item => item.Position)
            // order position groups by position
            .OrderBy(positionGroup => positionGroup.Key, orderer)
            // ordered by item text in the postion group
            .SelectMany(positionGroup => positionGroup.OrderBy(item => item.Text ?? ""));
    }

    private static MenuItem Join(IEnumerable<MenuItem> items)
    {
        if (items.Count() < 2)
        {
            return items.Single();
        }

        var joined = new MenuItem
        {
            Text = items.First().Text,
            CssClass = items.Select(x => x.CssClass).FirstOrDefault(x => !string.IsNullOrEmpty(x)),
            Icons = items.Select(x => x.Icons).FirstOrDefault(x => !x.IsNullOrEmpty()),
            Url = items.Select(x => x.Url).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)),
            Href = items.Select(x => x.Href).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)),
            RouteValues = items.Select(x => x.RouteValues).FirstOrDefault(x => x != null),
            Items = Merge(items.Select(x => x.Items)).ToArray(),
            Position = SelectBestPositionValue(items.Select(x => x.Position)),
            Permissions = items.SelectMany(x => x.Permissions).Distinct(),
        };
        return joined;
    }

    private static string SelectBestPositionValue(IEnumerable<string> positions)
    {
        var comparer = new FlatPositionComparer();
        return positions.Aggregate(string.Empty, (agg, pos) =>
            string.IsNullOrEmpty(agg)
                ? pos
                : string.IsNullOrEmpty(pos)
                    ? agg
                    : comparer.Compare(agg, pos) < 0 ? agg : pos);
    }

    public string GetUrl(string menuItemUrl, RouteValueDictionary routeValueDictionary)
    {
        if (string.IsNullOrEmpty(menuItemUrl) && routeValueDictionary == null)
        {
            return null;
        }

        string url = !string.IsNullOrEmpty(menuItemUrl) ? menuItemUrl : urlHelper.RouteUrl(routeValueDictionary);

        if (!string.IsNullOrEmpty(url) &&
            !(url.StartsWith("http://") || url.StartsWith("https://") || url.StartsWith("javascript:") || url.StartsWith("#") || url.StartsWith("/")))
        {
            if (url.StartsWith("~/"))
            {
                url = url[2..];
            }
            string appPath = webHelper.WebRootPath;
            if (appPath == "/")
            {
                appPath = "";
            }
            url = string.Format("{0}/{1}", appPath, url);
        }
        return url;
    }

    #endregion INavigationManager Members
}