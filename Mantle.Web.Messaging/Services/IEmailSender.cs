using System.Text.RegularExpressions;
using Mantle.Web.Messaging.Configuration;
using MailKit.Net.Smtp;
using MimeKit;

namespace Mantle.Web.Messaging.Services
{
    public interface IEmailSender
    {
        void Send(MimeMessage mailMessage);

        void Send(string subject, string body, string toEmailAddress);
    }

    public class DefaultEmailSender : IEmailSender
    {
        private readonly SmtpSettings smtpSettings;
        private static readonly Regex regexValidEmail = new Regex(@"[\w-]+@([\w-]+\.)+[\w-]+", RegexOptions.Compiled);

        public DefaultEmailSender(SmtpSettings smtpSettings)
        {
            this.smtpSettings = smtpSettings;
        }

        public void Send(MimeMessage mailMessage)
        {
            using (var smtpClient = new SmtpClient())
            {
                if (smtpSettings != null && !string.IsNullOrEmpty(smtpSettings.Host))
                {
                    smtpClient.Connect(smtpSettings.Host, smtpSettings.Port, smtpSettings.EnableSsl);
                    smtpClient.Authenticate(smtpSettings.Username, smtpSettings.Password);

                    if (mailMessage.From == null && IsValidEmailAddress(smtpSettings.Username))
                    {
                        var displayName = mailMessage.Headers["FromDisplayName"];
                        if (string.IsNullOrEmpty(displayName))
                        {
                            displayName = smtpSettings.DisplayName;
                        }
                        mailMessage.From.Add(new MailboxAddress(smtpSettings.Username, displayName));
                    }
                }

                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }
        }

        public void Send(string subject, string body, string toEmailAddress)
        {
            var mailMessage = new MimeMessage
            {
                Subject = subject,
                //SubjectEncoding = Encoding.UTF8,
                Body = new TextPart("html") { Text = body },
                //BodyEncoding = Encoding.UTF8,
                //IsBodyHtml = true
            };
            mailMessage.To.Add(new MailboxAddress(toEmailAddress));
            Send(mailMessage);
        }

        public static bool IsValidEmailAddress(string mailAddress)
        {
            return !string.IsNullOrEmpty(mailAddress) && regexValidEmail.IsMatch(mailAddress);
        }
    }
}