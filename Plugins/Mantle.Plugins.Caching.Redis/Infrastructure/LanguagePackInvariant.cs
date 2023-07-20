using Mantle.Localization;

namespace Mantle.Plugins.Caching.Redis.Infrastructure
{
    public class LanguagePackInvariant : ILanguagePack
    {
        #region ILanguagePack Members

        public string CultureCode => null;

        public IDictionary<string, string> LocalizedStrings => new Dictionary<string, string>
        {
            { LocalizableStrings.Settings.ConnectionString, "localhost,allowAdmin=true" },
        };

        #endregion ILanguagePack Members
    }
}