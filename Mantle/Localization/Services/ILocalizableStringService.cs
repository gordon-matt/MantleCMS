using Mantle.Caching;
using Mantle.Data;
using Mantle.Data.Services;
using Mantle.Localization.Domain;

namespace Mantle.Localization.Services
{
    public interface ILocalizableStringService : IGenericDataService<LocalizableString>
    {
    }

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