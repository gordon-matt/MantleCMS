using Extenso.Data.Entity;
using Mantle.Data.Entity;

namespace Mantle.Tenants.Domain
{
    public interface ITenantEntity : IEntity
    {
        int? TenantId { get; set; }
    }

    public class TenantEntity<T> : MantleBaseEntity<T>, ITenantEntity
    {
        public int? TenantId { get; set; }
    }
}