using Mantle.Caching;
using Mantle.Data;
using Mantle.Data.Services;
using Mantle.Messaging.Data.Domain;

namespace Mantle.Messaging.Services
{
    public interface IQueuedEmailService : IGenericDataService<QueuedEmail>
    {
    }

    public class QueuedEmailService : GenericDataService<QueuedEmail>, IQueuedEmailService
    {
        public QueuedEmailService(ICacheManager cacheManager, IRepository<QueuedEmail> repository)
            : base(cacheManager, repository)
        {
        }
    }
}