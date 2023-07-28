using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Domain;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;

public interface IZoneService : IGenericDataService<Zone>
{
}

public class ZoneService : GenericDataService<Zone>, IZoneService
{
    public ZoneService(ICacheManager cacheManager, IRepository<Zone> repository)
        : base(cacheManager, repository)
    {
    }
}