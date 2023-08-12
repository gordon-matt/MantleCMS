using Mantle.Web.Common.Areas.Admin.Regions.Entities;

namespace Mantle.Web.Common.Areas.Admin.Regions.Services;

public interface IRegionSettingsService : IGenericDataService<RegionSettings>
{
    /// <summary>
    /// Determines whethere there are any region settings of the specified type in the database.
    /// </summary>
    /// <typeparam name="T">The type of region settings</typeparam>
    /// <returns>True if there are any records of the specified type. Otherwise, false.</returns>
    bool SettingsExist<T>() where T : IRegionSettings;

    T GetSettings<T>(int regionId) where T : IRegionSettings;

    Dictionary<int, T> GetSettings<T>(IEnumerable<int> regionIds = null) where T : IRegionSettings;
}

public class RegionSettingsService : GenericDataService<RegionSettings>, IRegionSettingsService
{
    public RegionSettingsService(ICacheManager cacheManager, IRepository<RegionSettings> repository)
        : base(cacheManager, repository)
    {
    }

    public bool SettingsExist<T>() where T : IRegionSettings
    {
        var instance = Activator.CreateInstance<T>();
        string settingsId = instance.Name.ToSlugUrl();

        using var connection = OpenConnection();
        return connection.Query(x => x.SettingsId == settingsId).Any();
    }

    public T GetSettings<T>(int regionId) where T : IRegionSettings
    {
        var instance = Activator.CreateInstance<T>();
        string settingsId = instance.Name.ToSlugUrl();

        var settings = FindOne(x =>
            x.SettingsId == settingsId &&
            x.RegionId == regionId);

        if (settings == null)
        {
            return default;
        }

        return settings.Fields.JsonDeserialize<T>();
    }

    public Dictionary<int, T> GetSettings<T>(IEnumerable<int> regionIds = null) where T : IRegionSettings
    {
        var instance = Activator.CreateInstance<T>();
        string settingsId = instance.Name.ToSlugUrl();

        IEnumerable<RegionSettings> settings;

        if (regionIds.IsNullOrEmpty())
        {
            settings = Find(x => x.SettingsId == settingsId);
        }
        else
        {
            settings = Find(x =>
               x.SettingsId == settingsId &&
               regionIds.Contains(x.RegionId));
        }

        return settings.ToDictionary(k => k.RegionId, v => v.Fields.JsonDeserialize<T>());
    }
}