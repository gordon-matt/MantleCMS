using Extenso;
using Extenso.Data.Entity;
using Mantle.Infrastructure;
using Mantle.Web.Configuration.Services;

namespace Mantle.Web.Configuration.Domain
{
    public static class GenericAttributeExtensions
    {
        public static TPropType GetAttribute<TPropType>(this IEntity entity, string key, IGenericAttributeService service = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (service == null)
            {
                service = EngineContext.Current.Resolve<IGenericAttributeService>();
            }

            string entityId = string.Join("|", entity.KeyValues);
            string entityType = entity.GetType().Name;

            var props = service.GetAttributesForEntity(entityId, entityType);
            //little hack here (only for unit testing). we should write expect-return rules in unit tests for such cases
            if (props == null)
            {
                return default(TPropType);
            }

            if (!props.Any())
            {
                return default(TPropType);
            }

            var prop = props.FirstOrDefault(x => x.Property.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (prop == null || string.IsNullOrEmpty(prop.Value))
            {
                return default(TPropType);
            }

            return prop.Value.ConvertTo<TPropType>();
        }
    }
}