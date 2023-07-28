using Mantle.Web.Messaging.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Mantle.Web.Messaging.Services;

public interface IEmailSender
{
    void Send(MailMessage mailMessage);

    void Send(string subject, string body, string toEmailAddress);
}

public class DefaultEmailSender : IEmailSender
{
    private readonly SmtpSettings smtpSettings;
    private static readonly Regex regexValidEmail = new(@"[\w-]+@([\w-]+\.)+[\w-]+", RegexOptions.Compiled);

    public DefaultEmailSender(SmtpSettings smtpSettings)
    {
        this.smtpSettings = smtpSettings;
    }

    public void Send(MailMessage mailMessage)
    {
        using var smtpClient = new SmtpClient();
        if (smtpSettings != null && !string.IsNullOrEmpty(smtpSettings.Host))
        {
            smtpClient.UseDefaultCredentials = smtpSettings.UseDefaultCredentials;
            smtpClient.Host = smtpSettings.Host;
            smtpClient.Port = smtpSettings.Port;
            smtpClient.EnableSsl = smtpSettings.EnableSsl;
            smtpClient.Credentials = smtpSettings.UseDefaultCredentials
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(smtpSettings.Username, smtpSettings.Password);

            if (mailMessage.From == null && IsValidEmailAddress(smtpSettings.Username))
            {
                string displayName = mailMessage.Headers["FromDisplayName"];
                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = smtpSettings.DisplayName;
                }
                mailMessage.From = new MailAddress(smtpSettings.Username, displayName);
            }
        }

        smtpClient.Send(mailMessage);
    }

    public void Send(string subject, string body, string toEmailAddress)
    {
        var mailMessage = new MailMessage
        {
            Subject = subject,
            SubjectEncoding = Encoding.UTF8,
            Body = body,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmailAddress);
        Send(mailMessage);
    }

    public static bool IsValidEmailAddress(string mailAddress)
    {
        return !string.IsNullOrEmpty(mailAddress) && regexValidEmail.IsMatch(mailAddress);
    }
}