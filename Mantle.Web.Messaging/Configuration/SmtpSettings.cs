using Mantle.Localization.ComponentModel;
using Mantle.Web.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Mantle.Web.Messaging.Configuration
{
    public class SmtpSettings : ISettings
    {
        public SmtpSettings()
        {
            MaxTries = 3;
            MessagesPerBatch = 50;
        }

        #region ISettings Members

        public string Name => "SMTP Settings";

        public bool IsTenantRestricted => false;

        // Using an Embedded View in this case, since this assembly is not a plugin or the main app
        //TODO: Need to find a way to separate this: since it relies on the CMS project
        public string EditorTemplatePath => "Mantle.Web.Messaging.Views.Shared.EditorTemplates.SmtpSettings.cshtml";

        #endregion ISettings Members

        [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.DisplayName)]
        public string DisplayName { get; set; }

        [Required]
        [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.Host)]
        public string Host { get; set; }

        [Required]
        [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.Port)]
        public int Port { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.EnableSsl)]
        public bool EnableSsl { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.UseDefaultCredentials)]
        public bool UseDefaultCredentials { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.Username)]
        public string Username { get; set; }

        [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.Password)]
        public string Password { get; set; }

        [Required]
        [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.MaxTries)]
        public int MaxTries { get; set; }

        [Required]
        [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.MessagesPerBatch)]
        public int MessagesPerBatch { get; set; }
    }
}