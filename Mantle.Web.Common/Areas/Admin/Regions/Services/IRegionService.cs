using Mantle.Web.Common.Areas.Admin.Regions.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Web.Common.Areas.Admin.Regions.Services;

public interface IRegionService : IGenericDataService<Region>
{
    Region FindOne(int id, string cultureCode = null, bool includeChildren = false, bool includeParent = false);

    IEnumerable<Region> GetContinents(int tenantId, string cultureCode = null, bool includeCountries = false);

    IEnumerable<Region> GetSubRegions(int regionId, RegionType? regionType = null, string cultureCode = null);

    IEnumerable<Region> GetSubRegions(int regionId, int pageIndex, int pageSize, out int total, RegionType? regionType = null, string cultureCode = null);

    IEnumerable<Region> GetCountries(int tenantId, string cultureCode = null);

    IEnumerable<Region> GetStates(int countryId, string cultureCode = null, bool includeCities = false);
}

public class RegionService : GenericDataService<Region>, IRegionService
{
    private readonly Lazy<ILocalizablePropertyService> localizablePropertyService;

    public RegionService(
        ICacheManager cacheManager,
        IRepository<Region> repository,
        Lazy<ILocalizablePropertyService> localizablePropertyService)
        : base(cacheManager, repository)
    {
        this.localizablePropertyService = localizablePropertyService;
    }

    #region IRegionService Members

    public Region FindOne(int id, string cultureCode = null, bool includeChildren = false, bool includeParent = false)
    {
        using var connection = OpenConnection();
        var query = connection.Query();

        if (includeParent)
        {
            query = query.Include(x => x.Parent);
        }
        if (includeChildren)
        {
            query = query.Include(x => x.Children);
        }

        var region = query.First(x => x.Id == id);

        if (!string.IsNullOrEmpty(cultureCode))
        {
            string entityType = typeof(Region).FullName;
            string entityId = region.Id.ToString();

            var localizedRecord = localizablePropertyService.Value.FindOne(x =>
                x.CultureCode == cultureCode &&
                x.EntityType == entityType &&
                x.Property == "Name" &&
                x.EntityId == entityId);

            if (localizedRecord != null)
            {
                region.Name = localizedRecord.Value;
            }
        }

        return region;
    }

    public IEnumerable<Region> GetContinents(int tenantId, string cultureCode = null, bool includeCountries = false)
    {
        ICollection<Region> continents = null;

        using (var connection = OpenConnection())
        {
            var query = connection.Query(x =>
                x.RegionType == RegionType.Continent
                && x.TenantId == tenantId);

            if (includeCountries)
            {
                query = query.Include(x => x.Children);
            }

            continents = query
                .OrderBy(x => x.Order == null)
                .ThenBy(x => x.Order)
                .ThenBy(x => x.Name)
                .ToList();
        }

        if (!string.IsNullOrEmpty(cultureCode))
        {
            Localize(continents, cultureCode);
        }

        return continents;
    }

    public IEnumerable<Region> GetSubRegions(int regionId, RegionType? regionType = null, string cultureCode = null)
    {
        ICollection<Region> subRegions = null;

        using (var connection = OpenConnection())
        {
            if (regionType.HasValue)
            {
                subRegions = connection.Query()
                    .Include(x => x.Parent)
                    .Include(x => x.Children)
                    .Where(x => x.Parent.Id == regionId && x.RegionType == regionType)
                    .OrderBy(x => x.Order == null)
                    .ThenBy(x => x.Order)
                    .ThenBy(x => x.Name)
                    .ToHashSet();
            }
            else
            {
                subRegions = connection.Query()
                   .Include(x => x.Parent)
                   .Include(x => x.Children)
                   .Where(x => x.Parent.Id == regionId)
                   .OrderBy(x => x.Order == null)
                   .ThenBy(x => x.Order)
                   .ThenBy(x => x.Name)
                   .ToHashSet();
            }
        }

        if (!string.IsNullOrEmpty(cultureCode))
        {
            Localize(subRegions, cultureCode);
        }

        return subRegions;
    }

    public IEnumerable<Region> GetSubRegions(int regionId, int pageIndex, int pageSize, out int total, RegionType? regionType = null, string cultureCode = null)
    {
        using var connection = OpenConnection();
        IQueryable<Region> query = connection.Query()
            .Include(x => x.Parent)
            .Include(x => x.Children);

        if (regionType.HasValue)
        {
            query = query.Where(x => x.Parent.Id == regionId && x.RegionType == regionType);
        }
        else
        {
            query = query.Where(x => x.Parent.Id == regionId);
        }

        query = query
            .OrderBy(x => x.Order == null)
            .ThenBy(x => x.Order)
            .ThenBy(x => x.Name);

        total = query.Count();

        var subRegions = query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToHashSet();

        if (!string.IsNullOrEmpty(cultureCode))
        {
            Localize(subRegions, cultureCode);
        }

        return subRegions;
    }

    public IEnumerable<Region> GetCountries(int tenantId, string cultureCode = null)
    {
        using var connection = OpenConnection();
        var countries = connection.Query()
            .Include(x => x.Parent)
            .Include(x => x.Children)
            .Where(x =>
                x.RegionType == RegionType.Country
                && x.TenantId == tenantId)
            .OrderBy(x => x.Order == null)
            .ThenBy(x => x.Order)
            .ThenBy(x => x.Name)
            .ToHashSet();

        if (!string.IsNullOrEmpty(cultureCode))
        {
            Localize(countries, cultureCode);
        }

        return countries;
    }

    public IEnumerable<Region> GetStates(int countryId, string cultureCode = null, bool includeCities = false)
    {
        ICollection<Region> states = null;

        using (var connection = OpenConnection())
        {
            if (includeCities)
            {
                states = connection.Query()
                    .Include(x => x.Children)
                    .Where(x => x.ParentId == countryId && x.RegionType == RegionType.State)
                    .OrderBy(x => x.Order == null)
                    .ThenBy(x => x.Order)
                    .ThenBy(x => x.Name)
                    .ToHashSet();
            }
            else
            {
                states = connection.Query(x => x.ParentId == countryId && x.RegionType == RegionType.State)
                    .OrderBy(x => x.Order == null)
                    .ThenBy(x => x.Order)
                    .ThenBy(x => x.Name)
                    .ToHashSet();
            }
        }

        if (!string.IsNullOrEmpty(cultureCode))
        {
            Localize(states, cultureCode);
        }

        return states;
    }

    #endregion IRegionService Members

    private void Localize(ICollection<Region> regions, string cultureCode)
    {
        var regionIds = regions.Select(x => x.Id.ToString());

        string entityType = typeof(Region).FullName;

        var localizedRecords = localizablePropertyService.Value.Find(cultureCode, entityType, regionIds, "Name");

        foreach (var region in regions)
        {
            var localizedRecord = localizedRecords.FirstOrDefault(l => l.EntityId == region.Id.ToString());
            if (localizedRecord != null)
            {
                region.Name = localizedRecord.Value;
            }
        }
    }
}