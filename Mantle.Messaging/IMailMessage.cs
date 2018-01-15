using System.Net.Mail;

namespace Mantle.Messaging
{
    public interface IMailMessage
    {
        MailMessage GetMailMessage();
    }
}