using Extenso.Data.Entity;
using Mantle.Caching;
using Mantle.Data.Services;
using Mantle.Localization.Domain;

namespace Mantle.Localization.Services
{
    public class LocalizableStringService : GenericDataService<LocalizableString>, ILocalizableStringService
    {
        public LocalizableStringService(
            ICacheManager cacheManager,
            IRepository<LocalizableString> repository)
            : base(cacheManager, repository)
        {
        }
    }
}