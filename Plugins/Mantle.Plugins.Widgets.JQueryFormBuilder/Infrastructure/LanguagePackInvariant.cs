using System.Collections.Generic;
using Mantle.Localization;

namespace Mantle.Plugins.Widgets.JQueryFormBuilder.Infrastructure
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
                    { LocalizableStrings.ContentBlocks.FormBuilderBlock.EmailAddress, "Email Address" },
                    { LocalizableStrings.ContentBlocks.FormBuilderBlock.RedirectUrl, "Redirect URL (After Submit)" },
                    { LocalizableStrings.ContentBlocks.FormBuilderBlock.ThankYouMessage, "'Thank You' Message" },
                    { LocalizableStrings.ContentBlocks.FormBuilderBlock.UseAjax, "Use Ajax" },
                };
            }
        }

        #endregion ILanguagePack Members
    }
}