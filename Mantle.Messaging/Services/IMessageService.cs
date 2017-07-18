using System;
using System.Collections.Generic;
using System.Linq;
using Mantle.Caching;
using Mantle.Data;
using Mantle.Data.Services;
using Mantle.Messaging.Domain;
using Mantle.Web;
using MimeKit;

namespace Mantle.Messaging.Services
{
    public interface IMessageService : IGenericDataService<QueuedEmail>
    {
        Guid SendEmailMessage(int tenantId, string messageTemplate, IEnumerable<Token> tokens, string toEmailAddress, string toName = null);

        Guid SendEmailMessage(int tenantId, string subject, string body, string toEmailAddress, string toName = null);

        Guid SendEmailMessage(int tenantId, MimeMessage mailMessage);
    }

    public class MessageService : GenericDataService<QueuedEmail>, IMessageService, IQueuedMessageProvider
    {
        private readonly IMessageTemplateService messageTemplateService;
        private readonly ITokenizer tokenizer;
        private readonly IEnumerable<IMessageTokensProvider> tokenProviders;

        public MessageService(
            ICacheManager cacheManager,
            IMessageTemplateService messageTemplateService,
            IRepository<QueuedEmail> queuedEmailRepository,
            ITokenizer tokenizer,
            IEnumerable<IMessageTokensProvider> tokenProviders)
            : base(cacheManager, queuedEmailRepository)
        {
            this.tokenizer = tokenizer;
            this.tokenProviders = tokenProviders;
            this.messageTemplateService = messageTemplateService;
        }

        public Guid SendEmailMessage(int tenantId, string messageTemplate, IEnumerable<Token> tokens, string toEmailAddress, string toName = null)
        {
            var template = messageTemplateService.Find(tenantId, messageTemplate);
            if (template == null || !template.Enabled)
            {
                return Guid.Empty;
            }

            foreach (var tokenProvider in tokenProviders)
            {
                tokenProvider.GetTokens(messageTemplate, tokens);
            }

            return SendMessage(tenantId, template, tokens, toEmailAddress, toName);
        }

        public Guid SendEmailMessage(int tenantId, string subject, string body, string toEmailAddress, string toName = null)
        {
            var mailMessage = new MimeMessage
            {
                //SubjectEncoding = Encoding.UTF8,
                //BodyEncoding = Encoding.UTF8,
                Subject = subject,
                Body = new TextPart("html") { Text = body },
                //IsBodyHtml = true
            };
            mailMessage.To.Add(new MailboxAddress(toEmailAddress, toName));

            return SendEmailMessage(tenantId, mailMessage);
        }

        public Guid SendEmailMessage(int tenantId, MimeMessage mailMessage)
        {
            var address = mailMessage.To[0] as MailboxAddress;
            var queuedEmail = new QueuedEmail
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Priority = 5,
                ToAddress = address.Address,
                ToName = address.Name,
                Subject = mailMessage.Subject,
                MailMessage = mailMessage.ToJson(),
                CreatedOnUtc = DateTime.UtcNow
            };

            Insert(queuedEmail);

            return queuedEmail.Id;
        }

        private Guid SendMessage(
            int tenantId,
            MessageTemplate messageTemplate,
            IEnumerable<Token> tokens,
            string toEmailAddress,
            string toName)
        {
            var subject = messageTemplate.Subject ?? string.Empty;
            var body = messageTemplate.Body ?? string.Empty;

            //Replace subject and body tokens
            var subjectReplaced = tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = tokenizer.Replace(body, tokens, true);

            return SendEmailMessage(tenantId, subjectReplaced, bodyReplaced, toEmailAddress, toName);
        }

        public IEnumerable<IMailMessage> GetQueuedEmails(int tenantId, int maxSendTries, int maxMessageItems)
        {
            using (var connection = OpenConnection())
            {
                return connection
                    .Query(x =>
                        x.TenantId == tenantId
                        && x.SentTries < maxSendTries
                        && x.SentOnUtc == null)
                    .OrderBy(x => x.Priority)
                    .ThenBy(x => x.CreatedOnUtc)
                    .Take(maxMessageItems)
                    .ToList();
            }
        }

        public void OnSendSuccess(IMailMessage mailMessage)
        {
            var entity = (QueuedEmail)mailMessage;
            entity.SentOnUtc = DateTime.UtcNow;
            Update(entity);
        }

        public void OnSendError(IMailMessage mailMessage)
        {
            var entity = (QueuedEmail)mailMessage;
            entity.SentTries++;
            Update(entity);
        }
    }
}