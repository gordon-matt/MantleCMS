using System.Collections.Generic;
using System.Linq;
using Mantle.Web.ContentManagement.Areas.Admin.Messaging;

namespace Mantle.Web.ContentManagement.Areas.Admin.Newsletters
{
    public class NewsletterMessageTemplates : IMessageTemplatesProvider
    {
        public const string Newsletter_Subscribed = "Newsletter: Subscribed";
        public const string Newsletter_Unsubscribed = "Newsletter: Unsubscribed";

        #region IMessageTemplatesProvider Members

        public IEnumerable<MessageTemplate> GetTemplates()
        {
            yield return new MessageTemplate(Newsletter_Subscribed, "Thank you for subscribing!");
            yield return new MessageTemplate(Newsletter_Unsubscribed, "You have unsubscribed from our mailing list.");
        }

        #endregion IMessageTemplatesProvider Members

        #region IMessageTokensProvider Members

        public IEnumerable<string> GetAvailableTokens(string template)
        {
            return Enumerable.Empty<string>();
        }

        public void GetTokens(string template, IEnumerable<Token> tokens)
        {
        }

        #endregion IMessageTokensProvider Members
    }
}