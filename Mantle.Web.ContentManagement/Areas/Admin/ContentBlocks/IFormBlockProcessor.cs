using System.Net.Mail;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

public interface IFormBlockProcessor
{
    void Process(FormCollection formCollection, MailMessage mailMessage);
}