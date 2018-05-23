﻿using Extenso.Data.Entity;
using Mantle.Caching;
using Mantle.Data.Services;
using Mantle.Messaging.Data.Domain;

namespace Mantle.Messaging.Services
{
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

        public MessageTemplateVersion FindOne(int templateId, string cultureCode)
        {
            return FindOne(x => x.MessageTemplateId == templateId && x.CultureCode == cultureCode);
        }
    }
}