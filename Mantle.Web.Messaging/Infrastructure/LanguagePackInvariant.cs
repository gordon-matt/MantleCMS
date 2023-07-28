using Mantle.Localization;

namespace Mantle.Web.Messaging.Infrastructure;

public class LanguagePackInvariant : ILanguagePack
{
    #region ILanguagePack Members

    public string CultureCode => null;

    public IDictionary<string, string> LocalizedStrings => new Dictionary<string, string>
    {
        { LocalizableStrings.GetTokensError, "Could not get tokens." },
        { LocalizableStrings.MessageTemplate.Editor, "Editor" },
        { LocalizableStrings.MessageTemplate.Name, "Name" },
        { LocalizableStrings.MessageTemplate.Tokens, "Tokens" },
        { LocalizableStrings.MessageTemplates, "Message Templates" },
        { LocalizableStrings.MessageTemplateVersion.Data, "Body" },
        { LocalizableStrings.MessageTemplateVersion.Subject, "Subject" },
        { LocalizableStrings.Messaging, "Messaging" },
        { LocalizableStrings.QueuedEmail.CreatedOnUtc, "Created On (UTC)" },
        { LocalizableStrings.QueuedEmail.SentOnUtc, "Sent On (UTC)" },
        { LocalizableStrings.QueuedEmail.SentTries, "Sent Tries" },
        { LocalizableStrings.QueuedEmail.Subject, "Subject" },
        { LocalizableStrings.QueuedEmail.ToAddress, "To Address" },
        { LocalizableStrings.QueuedEmails, "Queued Emails" },

        { LocalizableStrings.Settings.Smtp.DisplayName, "Display Name" },
        { LocalizableStrings.Settings.Smtp.EnableSsl, "Enable SSL" },
        { LocalizableStrings.Settings.Smtp.Host, "Host" },
        { LocalizableStrings.Settings.Smtp.MaxTries, "Max Tries" },
        { LocalizableStrings.Settings.Smtp.MessagesPerBatch, "Messages Per Batch" },
        { LocalizableStrings.Settings.Smtp.Password, "Password" },
        { LocalizableStrings.Settings.Smtp.Port, "Port" },
        { LocalizableStrings.Settings.Smtp.UseDefaultCredentials, "Use Default Credentials" },
        { LocalizableStrings.Settings.Smtp.Username, "Username" },
    };

    #endregion ILanguagePack Members
}