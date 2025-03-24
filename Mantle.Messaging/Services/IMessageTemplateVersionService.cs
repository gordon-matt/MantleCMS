namespace Mantle.Messaging.Services;

public interface IMessageTemplateVersionService : IGenericDataService<MessageTemplateVersion>
{
    MessageTemplateVersion FindOne(int templateId, string cultureCode);
}

public class MessageTemplateVersionService : GenericDataService<MessageTemplateVersion>, IMessageTemplateVersionService
{
    public MessageTemplateVersionService(ICacheManager cacheManager, IRepository<MessageTemplateVersion> repository)
        : base(cacheManager, repository)
    {
    }

    public MessageTemplateVersion FindOne(int templateId, string cultureCode) =>
        FindOne(x => x.MessageTemplateId == templateId && x.CultureCode == cultureCode);
}