using Mantle.Helpers;

namespace Mantle.Web.Mvc.Themes;

public partial class ThemeProvider : IThemeProvider
{
    #region Fields

    private readonly IList<ThemeConfiguration> themeConfigurations = [];
    private readonly string basePath = string.Empty;

    #endregion Fields

    #region Constructors

    public ThemeProvider()
    {
        basePath = CommonHelper.MapPath("~/Themes");
        LoadConfigurations();
    }

    #endregion Constructors

    #region IThemeProvider

    public ThemeConfiguration GetThemeConfiguration(string themeName) => themeConfigurations
        .SingleOrDefault(x => x.ThemeName.Equals(themeName, StringComparison.OrdinalIgnoreCase));

    public IList<ThemeConfiguration> GetThemeConfigurations() => themeConfigurations;

    public bool ThemeConfigurationExists(string themeName) => GetThemeConfigurations().Any(configuration => configuration.ThemeName.Equals(themeName, StringComparison.OrdinalIgnoreCase));

    #endregion IThemeProvider

    #region Utility

    private void LoadConfigurations()
    {
        if (!Directory.Exists(basePath))
        {
            return;
        }

        foreach (string themeName in Directory.GetDirectories(basePath))
        {
            var configuration = CreateThemeConfiguration(themeName);
            if (configuration != null)
            {
                themeConfigurations.Add(configuration);
            }
        }
    }

    private ThemeConfiguration CreateThemeConfiguration(string themePath)
    {
        var themeDirectory = new DirectoryInfo(themePath);
        var themeConfigFile = new FileInfo(Path.Combine(themeDirectory.FullName, "theme.config"));

        return themeConfigFile.Exists ? ThemeConfiguration.Load(themeConfigFile.FullName, themeDirectory.Name) : null;
    }

    #endregion Utility
}