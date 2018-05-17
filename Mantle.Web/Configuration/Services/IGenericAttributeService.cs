using System;
using System.Collections.Generic;
using System.Linq;
using Mantle.Caching;
using Mantle.Data;
using Mantle.Data.Services;
using Extenso.Reflection;
using Mantle.Web.Configuration.Domain;
using Extenso;

namespace Mantle.Web.Configuration.Services
{
    public interface IGenericAttributeService : IGenericDataService<GenericAttribute>
    {
        IEnumerable<GenericAttribute> GetAttributesForEntity(string entityId, string entityType);

        TPropType GetAttribute<TPropType>(IEntity entity, string property);

        void SaveAttribute<TPropType>(IEntity entity, string property, TPropType value);
    }

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
            return CacheManager.Get(cacheKey, () =>
            {
                return Find(x => x.EntityId == entityId && x.EntityType == entityType);
            });
        }

        public virtual TPropType GetAttribute<TPropType>(IEntity entity, string property)
        {
            string entityType = entity.GetType().Name;
            string entityId = string.Join("|", entity.KeyValues);
            var props = GetAttributesForEntity(entityId, entityType);

            if (props == null)
            {
                return default(TPropType);
            }

            var prop = props.FirstOrDefault(ga =>
                ga.Property.Equals(property, StringComparison.OrdinalIgnoreCase));

            if (prop == null || string.IsNullOrEmpty(prop.Value))
            {
                return default(TPropType);
            }

            if (typeof(TPropType).IsSimple())
            {
                return prop.Value.ConvertTo<TPropType>();
            }
            else
            {
                return prop.Value.JsonDeserialize<TPropType>();
            }
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

            string valueStr = null;
            if (typeof(TPropType).IsSimple())
            {
                valueStr = value.ToString();
            }
            else
            {
                valueStr = value.ToJson();
            }

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
}