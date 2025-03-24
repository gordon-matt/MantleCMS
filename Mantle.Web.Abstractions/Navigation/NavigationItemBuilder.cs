namespace Mantle.Web.Navigation;

public class NavigationItemBuilder : NavigationBuilder
{
    private readonly MenuItem item;

    public NavigationItemBuilder()
    {
        item = new MenuItem();
    }

    public NavigationItemBuilder Caption(string caption)
    {
        item.Text = caption;
        return this;
    }

    public NavigationItemBuilder Position(string position)
    {
        item.Position = position;
        return this;
    }

    public NavigationItemBuilder Url(string url)
    {
        item.Url = url;
        return this;
    }

    //public NavigationItemBuilder Description(string description)
    //{
    //    item.Description = description;
    //    return this;
    //}

    public NavigationItemBuilder CssClass(string className)
    {
        item.CssClass = className;
        return this;
    }

    //public NavigationItemBuilder IconCssClass(string className)
    //{
    //    item.IconCssClass = className;
    //    return this;
    //}

    public NavigationItemBuilder Icons(params string[] icons)
    {
        item.Icons = item.Icons.Concat(icons);
        return this;
    }

    public override IEnumerable<MenuItem> Build()
    {
        item.Items = base.Build();
        return [item];
    }

    public NavigationItemBuilder Action(RouteValueDictionary routeValues) => routeValues != null
        ? Action(routeValues["action"] as string, routeValues["controller"] as string, routeValues)
        : Action(null, null, []);

    public NavigationItemBuilder Action(string actionName) => Action(actionName, null, []);

    public NavigationItemBuilder Action(string actionName, string controllerName) => Action(actionName, controllerName, []);

    public NavigationItemBuilder Action(string actionName, string controllerName, object routeValues) => Action(actionName, controllerName, new RouteValueDictionary(routeValues));

    public NavigationItemBuilder Action(string actionName, string controllerName, RouteValueDictionary routeValues)
    {
        item.RouteValues = new RouteValueDictionary(routeValues);
        if (!string.IsNullOrEmpty(actionName))
        {
            item.RouteValues["action"] = actionName;
        }
        if (!string.IsNullOrEmpty(controllerName))
        {
            item.RouteValues["controller"] = controllerName;
        }
        return this;
    }

    public NavigationItemBuilder Permission(params Permission[] permissions)
    {
        item.Permissions = item.Permissions.Concat(permissions);
        return this;
    }
}