using MimeKit;

namespace Mantle.Messaging
{
    public interface IMailMessage
    {
        MimeMessage GetMailMessage();
    }
}