namespace Mantle.Messaging.Services;

public interface IMessageService : IGenericDataService<QueuedEmail>
{
    //Guid SendEmailMessage(
    //    int tenantId,
    //    string cultureCode,
    //    string messageTemplate,
    //    IEnumerable<Token> tokens,
    //    string toEmailAddress,
    //    string toName = null);

    Guid SendEmailMessage(
        int tenantId,
        string subject,
        string body,
        string toEmailAddress,
        string toName = null);

    Guid SendEmailMessage(int tenantId, MailMessage mailMessage);
}

public class MessageService : GenericDataService<QueuedEmail>, IMessageService, IQueuedMessageProvider
{
    private readonly IEnumerable<IMessageTokensProvider> tokenProviders;
    private readonly IMessageTemplateService messageTemplateService;
    private readonly IMessageTemplateVersionService messageTemplateVersionService;
    private readonly ITokenizer tokenizer;

    public MessageService(
        ICacheManager cacheManager,
        IEnumerable<IMessageTokensProvider> tokenProviders,
        IMessageTemplateService messageTemplateService,
        IMessageTemplateVersionService messageTemplateVersionService,
        IRepository<QueuedEmail> queuedEmailRepository,
        ITokenizer tokenizer)
        : base(cacheManager, queuedEmailRepository)
    {
        this.messageTemplateService = messageTemplateService;
        this.messageTemplateVersionService = messageTemplateVersionService;
        this.tokenizer = tokenizer;
        this.tokenProviders = tokenProviders;
    }

    #region IMessageService Members

    //public Guid SendEmailMessage(
    //    int tenantId,
    //    string cultureCode,
    //    string messageTemplate,
    //    IEnumerable<Token> tokens,
    //    string toEmailAddress,
    //    string toName = null)
    //{
    //    var template = messageTemplateService.Find(tenantId, messageTemplate);
    //    var version = messageTemplateVersionService.FindOne(template.Id, cultureCode);

    //    if (version == null || !version.Enabled)
    //    {
    //        return Guid.Empty;
    //    }

    //    foreach (var tokenProvider in tokenProviders)
    //    {
    //        tokenProvider.GetTokens(messageTemplate, tokens);
    //    }

    //    return SendMessage(tenantId, version, tokens, toEmailAddress, toName);
    //}

    public Guid SendEmailMessage(
        int tenantId,
        string subject,
        string body,
        string toEmailAddress,
        string toName = null)
    {
        var mailMessage = new MailMessage
        {
            SubjectEncoding = Encoding.UTF8,
            BodyEncoding = Encoding.UTF8,
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(new MailAddress(toEmailAddress, toName));

        return SendEmailMessage(tenantId, mailMessage);
    }

    public Guid SendEmailMessage(int tenantId, MailMessage mailMessage)
    {
        var mailMessageWrap = new MailMessageWrapper(mailMessage);

        var queuedEmail = new QueuedEmail
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Priority = 5,
            ToAddress = mailMessage.To[0].Address,
            ToName = mailMessage.To[0].DisplayName,
            Subject = mailMessage.Subject,
            MailMessage = mailMessageWrap.ToString(),
            CreatedOnUtc = DateTime.UtcNow
        };

        Insert(queuedEmail);

        return queuedEmail.Id;
    }

    #endregion IMessageService Members

    #region IQueuedMessageProvider Members

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

    #endregion IQueuedMessageProvider Members

    //private Guid SendMessage(
    //    int tenantId,
    //    MessageTemplateVersion messageTemplateVersion,
    //    IEnumerable<Token> tokens,
    //    string toEmailAddress,
    //    string toName)
    //{
    //    var subject = messageTemplateVersion.Subject ?? string.Empty;

    //    var data = messageTemplateVersion.Data.JsonDeserialize<GrapesJsStorageData>();

    //    var body = messageTemplateVersion.Body ?? string.Empty;

    //    //Replace subject and body tokens
    //    var subjectReplaced = tokenizer.Replace(subject, tokens, false);
    //    var bodyReplaced = tokenizer.Replace(body, tokens, true);

    //    return SendEmailMessage(tenantId, subjectReplaced, bodyReplaced, toEmailAddress, toName);
    //}
}