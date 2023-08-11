using System.ComponentModel.DataAnnotations;

namespace Mantle.Web.Messaging.Configuration;

public class SmtpSettings : BaseSettings
{
    public SmtpSettings()
    {
        MaxTries = 3;
        MessagesPerBatch = 50;
    }

    #region ISettings Members

    public override string Name => "Messaging: SMTP Settings";

    public override string EditorTemplatePath => "/Views/Shared/EditorTemplates/SmtpSettings.cshtml";

    #endregion ISettings Members

    [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.DisplayName)]
    [SettingsProperty]
    public string DisplayName { get; set; }

    [Required]
    [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.Host)]
    [SettingsProperty]
    public string Host { get; set; }

    [Required]
    [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.Port)]
    [SettingsProperty]
    public int Port { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.EnableSsl)]
    [SettingsProperty]
    public bool EnableSsl { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.UseDefaultCredentials)]
    [SettingsProperty]
    public bool UseDefaultCredentials { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.Username)]
    [SettingsProperty]
    public string Username { get; set; }

    [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.Password)]
    [SettingsProperty]
    public string Password { get; set; }

    [Required]
    [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.MaxTries)]
    [SettingsProperty(3)]
    public int MaxTries { get; set; }

    [Required]
    [LocalizedDisplayName(LocalizableStrings.Settings.Smtp.MessagesPerBatch)]
    [SettingsProperty(50)]
    public int MessagesPerBatch { get; set; }
}