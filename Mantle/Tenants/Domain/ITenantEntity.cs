using Mantle.Data;

namespace Mantle.Tenants.Domain
{
    public interface ITenantEntity : IEntity
    {
        int? TenantId { get; set; }
    }
}