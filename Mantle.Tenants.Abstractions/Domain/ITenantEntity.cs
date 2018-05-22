using Extenso.Data.Entity;

namespace Mantle.Tenants.Domain
{
    public interface ITenantEntity : IEntity
    {
        int? TenantId { get; set; }
    }
}