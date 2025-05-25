namespace Mantle.Web.Configuration.Services;

public class GenericAttributeService : GenericDataService<GenericAttribute>, IGenericAttributeService
{
    private const string GenericAttributeKeyFormat = "Mantle.GenericAttribute.{0}-{1}";
    private const string GenericAttributePatternKey = "Mantle.GenericAttribute.*";

    public GenericAttributeService(ICacheManager cacheManager, IRepository<GenericAttribute> repository)
        : base(cacheManager, repository)
    {
    }

    public virtual IEnumerable<GenericAttribute> GetAttributesForEntity(string entityId, string entityType)
    {
        string cacheKey = string.Format(GenericAttributeKeyFormat, entityId, entityType);
        return CacheManager.Get(cacheKey, () => Find(new SearchOptions<GenericAttribute>
        {
            Query = x => x.EntityId == entityId && x.EntityType == entityType
        }));
    }

    public virtual TPropType GetAttribute<TPropType>(IEntity entity, string property)
    {
        string entityType = entity.GetType().Name;
        string entityId = string.Join("|", entity.KeyValues);
        var props = GetAttributesForEntity(entityId, entityType);

        if (props == null)
        {
            return default;
        }

        var prop = props.FirstOrDefault(ga =>
            ga.Property.Equals(property, StringComparison.OrdinalIgnoreCase));

        return prop == null || string.IsNullOrEmpty(prop.Value)
            ? default
            : typeof(TPropType).IsSimple() ? prop.Value.ConvertTo<TPropType>() : prop.Value.JsonDeserialize<TPropType>();
    }

    public virtual void SaveAttribute<TPropType>(IEntity entity, string property, TPropType value)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (property == null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        string entityId = string.Join("|", entity.KeyValues);
        string entityType = entity.GetType().Name;

        var props = GetAttributesForEntity(entityId, entityType).ToList();

        var prop = props.FirstOrDefault(x =>
            x.Property.Equals(property, StringComparison.OrdinalIgnoreCase));

        string valueStr = typeof(TPropType).IsSimple() ? value.ToString() : value.JsonSerialize();
        if (prop != null)
        {
            if (string.IsNullOrWhiteSpace(valueStr))
            {
                //delete
                Delete(prop);
            }
            else
            {
                //update
                prop.Value = valueStr;
                Update(prop);
            }
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(valueStr))
            {
                //insert
                prop = new GenericAttribute
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    Property = property,
                    Value = valueStr
                };
                Insert(prop);
            }
        }
    }

    protected override void ClearCache()
    {
        base.ClearCache();
        CacheManager.RemoveByPattern(GenericAttributePatternKey); // TODO: Test
    }
}