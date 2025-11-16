using Mantle.Web.Configuration.Services;

namespace Mantle.Web.Configuration.Entities;

public static class GenericAttributeExtensions
{
    extension(IEntity entity)
    {
        public TPropType GetAttribute<TPropType>(string key, IGenericAttributeService service = null)
        {
            ArgumentNullException.ThrowIfNull(entity);

            service ??= DependoResolver.Instance.Resolve<IGenericAttributeService>();

            string entityId = string.Join("|", entity.KeyValues);
            string entityType = entity.GetType().Name;

            var props = service.GetAttributesForEntity(entityId, entityType);
            //little hack here (only for unit testing). we should write expect-return rules in unit tests for such cases
            if (props is null)
            {
                return default;
            }

            if (!props.Any())
            {
                return default;
            }

            var prop = props.FirstOrDefault(x => x.Property.Equals(key, StringComparison.OrdinalIgnoreCase));

            return prop is null || string.IsNullOrEmpty(prop.Value)
                ? default
                : prop.Value.ConvertTo<TPropType>();
        }
    }
}