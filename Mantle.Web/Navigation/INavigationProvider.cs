namespace Mantle.Web.Navigation;

public interface INavigationProvider
{
    string MenuName { get; }

    void GetNavigation(NavigationBuilder builder);
}