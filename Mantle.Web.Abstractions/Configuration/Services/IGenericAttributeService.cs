namespace Mantle.Web.Configuration.Services;

public interface IGenericAttributeService : IGenericDataService<GenericAttribute>
{
    IEnumerable<GenericAttribute> GetAttributesForEntity(string entityId, string entityType);

    TPropType GetAttribute<TPropType>(IEntity entity, string property);

    void SaveAttribute<TPropType>(IEntity entity, string property, TPropType value);
}