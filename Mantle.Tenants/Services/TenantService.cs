using Extenso.Data.Entity;
using Mantle.Caching;
using Mantle.Data.Services;
using Mantle.Helpers;
using Mantle.Tenants.Entities;

namespace Mantle.Tenants.Services;

public class TenantService : GenericDataService<Tenant>, ITenantService
{
    public TenantService(ICacheManager cacheManager, IRepository<Tenant> repository)
        : base(cacheManager, repository)
    {
    }

    public void EnsureTenantMediaFolderExists(int tenantId)
    {
        var mediaFolder = new DirectoryInfo(CommonHelper.MapPath("~/wwwroot/Media/Uploads/Tenant_" + tenantId));
        if (!mediaFolder.Exists)
        {
            mediaFolder.Create();
        }
    }
}