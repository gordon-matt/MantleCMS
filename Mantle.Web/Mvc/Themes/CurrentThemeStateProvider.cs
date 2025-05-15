namespace Mantle.Web.Mvc.Themes;

public class CurrentThemeStateProvider : IWorkContextStateProvider
{
    public Func<IWorkContext, T> Get<T>(string name)
    {
        if (name == MantleWebConstants.StateProviders.CurrentTheme)
        {
            string currentTheme = DependoResolver.Instance.Resolve<IThemeContext>().WorkingTheme;
            return ctx => (T)(object)currentTheme;
        }
        return null;
    }
}