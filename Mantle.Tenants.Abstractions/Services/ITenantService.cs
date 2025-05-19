using Mantle.Data.Services;
using Mantle.Tenants.Entities;

namespace Mantle.Tenants.Services;

public interface ITenantService : IGenericDataService<Tenant>
{
    void EnsureTenantMediaFolderExists(int tenantId);
}