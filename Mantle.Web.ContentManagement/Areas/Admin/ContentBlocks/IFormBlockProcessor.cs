using System.Net.Mail;
using Microsoft.AspNetCore.Http;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks
{
    public interface IFormBlockProcessor
    {
        void Process(FormCollection formCollection, MailMessage mailMessage);
    }
}