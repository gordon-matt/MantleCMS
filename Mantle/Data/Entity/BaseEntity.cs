using Mantle.Tenants.Domain;

namespace Mantle.Data.Entity
{
    public class BaseEntity<T> : IEntity
    {
        public T Id { get; set; }

        #region IEntity Members

        public object[] KeyValues => new object[] { Id };

        #endregion IEntity Members
    }

    public abstract class BaseTenantEntity<TKey> : BaseEntity<TKey>, ITenantEntity
    {
        public int? TenantId { get; set; }
    }
}