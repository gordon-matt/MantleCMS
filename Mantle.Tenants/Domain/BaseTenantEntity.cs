using Mantle.Data.Entity;

namespace Mantle.Tenants.Domain
{
    public abstract class BaseTenantEntity<TKey> : BaseEntity<TKey>, ITenantEntity
    {
        public int? TenantId { get; set; }
    }
}