using Mantle.Localization;

namespace MantleCMS.Infrastructure;

public class LanguagePackInvariant : ILanguagePack
{
    #region ILanguagePack Members

    public string CultureCode => null;

    public IDictionary<string, string> LocalizedStrings => new Dictionary<string, string>
    {
        { LocalizableStrings.Dashboard.Administration, "Administration" },
        { LocalizableStrings.Dashboard.Frontend, "Frontend" },
        { LocalizableStrings.Dashboard.Title, "Dashboard" },
        { LocalizableStrings.Dashboard.ToggleNavigation, "Toggle Navigation" },
    };

    #endregion ILanguagePack Members
}