using System.Collections.Generic;

namespace Mantle.Localization
{
    public interface ILanguagePack
    {
        /// <summary>
        /// Leave NULL for default (invariant) culture
        /// </summary>
        string CultureCode { get; }

        IDictionary<string, string> LocalizedStrings { get; }
    }
}