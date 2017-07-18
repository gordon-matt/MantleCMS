using System.Collections.Generic;
using Mantle.Localization;

namespace MantleCMS.Infrastructure
{
    public class LanguagePackInvariant : ILanguagePack
    {
        #region ILanguagePack Members

        public string CultureCode
        {
            get { return null; }
        }

        public IDictionary<string, string> LocalizedStrings
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { LocalizableStrings.Manage, "Manage" },
                };
            }
        }

        #endregion ILanguagePack Members
    }
}