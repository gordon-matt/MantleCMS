using Extenso.Data.Entity;

namespace Mantle.Tenants.Entities;

public interface ITenantEntity : IEntity
{
    int? TenantId { get; set; }
}

public class TenantEntity<T> : BaseEntity<T>, ITenantEntity
{
    public int? TenantId { get; set; }
}