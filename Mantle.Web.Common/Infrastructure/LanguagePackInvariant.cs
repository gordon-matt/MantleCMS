using System.Collections.Generic;
using Mantle.Localization;

namespace Mantle.Web.Common.Infrastructure
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
                    { LocalizableStrings.Regions.Cities, "Cities" },
                    { LocalizableStrings.Regions.States, "States" },
                    { LocalizableStrings.Regions.Title, "Regions" },
                    { LocalizableStrings.Regions.Model.CountryCode, "Country Code" },
                    { LocalizableStrings.Regions.Model.HasStates, "Has States" },
                    { LocalizableStrings.Regions.Model.Name, "Name" },
                    { LocalizableStrings.Regions.Model.Order, "Order" },
                    { LocalizableStrings.Regions.Model.RegionType, "Region Type" },
                    { LocalizableStrings.Regions.Model.StateCode, "State Code" },
                };
            }
        }

        #endregion ILanguagePack Members
    }
}