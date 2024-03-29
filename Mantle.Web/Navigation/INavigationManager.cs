﻿namespace Mantle.Web.Navigation;

public interface INavigationManager
{
    IEnumerable<MenuItem> BuildMenu(string menuName);

    string GetUrl(string menuItemUrl, RouteValueDictionary routeValueDictionary);
}