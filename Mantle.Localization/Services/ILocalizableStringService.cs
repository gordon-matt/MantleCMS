namespace Mantle.Localization.Services;

public class LocalizableStringService : GenericDataService<LocalizableString>, ILocalizableStringService
{
    public LocalizableStringService(
        ICacheManager cacheManager,
        IRepository<LocalizableString> repository)
        : base(cacheManager, repository)
    {
    }
}