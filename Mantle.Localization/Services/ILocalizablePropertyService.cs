namespace Mantle.Localization.Services;

public class LocalizablePropertyService : GenericDataService<LocalizableProperty>, ILocalizablePropertyService
{
    public LocalizablePropertyService(ICacheManager cacheManager, IRepository<LocalizableProperty> repository)
        : base(cacheManager, repository)
    {
    }

    public LocalizableProperty FindOne(string cultureCode, string entityType, string entityId, string property) =>
        FindOne(new SearchOptions<LocalizableProperty>
        {
            Query = x =>
                x.CultureCode == cultureCode &&
                x.EntityType == entityType &&
                x.Property == property &&
                x.EntityId == entityId
        });

    public IEnumerable<LocalizableProperty> Find(string cultureCode, string entityType, string entityId) =>
        Find(new SearchOptions<LocalizableProperty>
        {
            Query = x =>
                x.CultureCode == cultureCode &&
                x.EntityType == entityType &&
                x.EntityId == entityId
        });

    public IEnumerable<LocalizableProperty> Find(string cultureCode, string entityType, IEnumerable<string> entityIds) =>
        Find(new SearchOptions<LocalizableProperty>
        {
            Query = x =>
                x.CultureCode == cultureCode &&
                x.EntityType == entityType &&
                entityIds.Contains(x.EntityId)
        });

    public IEnumerable<LocalizableProperty> Find(string cultureCode, string entityType, IEnumerable<string> entityIds, string property) =>
        Find(new SearchOptions<LocalizableProperty>
        {
            Query = x =>
            x.CultureCode == cultureCode &&
            x.EntityType == entityType &&
            x.Property == property &&
            entityIds.Contains(x.EntityId)
        });
}