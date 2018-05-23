using Extenso.Data.Entity;
using Mantle.Caching;
using Mantle.Data.Services;
using Mantle.Tenants.Domain;

namespace Mantle.Tenants.Services
{
    public class TenantService : GenericDataService<Tenant>, ITenantService
    {
        public TenantService(ICacheManager cacheManager, IRepository<Tenant> repository)
            : base(cacheManager, repository)
        {
        }
    }
}