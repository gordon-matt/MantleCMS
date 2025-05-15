namespace Mantle.Web.Configuration.Services;

public class DefaultSettingService : ISettingService
{
    private readonly ICacheManager cacheManager;
    private readonly IRepository<Setting> repository;

    public DefaultSettingService(ICacheManager cacheManager, IRepository<Setting> repository)
    {
        this.cacheManager = cacheManager;
        this.repository = repository;
    }

    public TSettings GetSettings<TSettings>(int? tenantId = null) where TSettings : ISettings, new()
    {
        string type = typeof(TSettings).FullName;
        string key = string.Format(MantleWebConstants.CacheKeys.SettingsKeyFormat, tenantId, type);
        return cacheManager.Get(key, () =>
        {
            var settings = tenantId.HasValue
                ? repository.FindOne(x => x.TenantId == tenantId && x.Type == type)
                : repository.FindOne(x => x.TenantId == null && x.Type == type);
            return settings == null || string.IsNullOrEmpty(settings.Value) ? new TSettings() : settings.Value.JsonDeserialize<TSettings>();
        });
    }

    public ISettings GetSettings(Type settingsType, int? tenantId = null)
    {
        string type = settingsType.FullName;
        string key = string.Format(MantleWebConstants.CacheKeys.SettingsKeyFormat, tenantId, type);
        return cacheManager.Get(key, () =>
        {
            var settings = tenantId.HasValue
                ? repository.FindOne(x => x.TenantId == tenantId && x.Type == type)
                : repository.FindOne(x => x.TenantId == null && x.Type == type);
            return settings == null || string.IsNullOrEmpty(settings.Value)
                ? (ISettings)Activator.CreateInstance(settingsType)
                : (ISettings)settings.Value.JsonDeserialize(settingsType);
        });
    }

    public void SaveSettings(string key, string value, int? tenantId = null)
    {
        var setting = tenantId.HasValue
            ? repository.FindOne(x => x.TenantId == tenantId && x.Type == key)
            : repository.FindOne(x => x.TenantId == null && x.Type == key);
        if (setting == null)
        {
            var iSettings = DependoResolver.Instance.ResolveAll<ISettings>().FirstOrDefault(x => x.GetType().FullName == key);

            if (iSettings != null)
            {
                setting = new Setting { TenantId = tenantId, Name = iSettings.Name, Type = key, Value = value };
                repository.Insert(setting);
                cacheManager.RemoveByPattern(string.Format(MantleWebConstants.CacheKeys.SettingsKeysPatternFormat, tenantId));
            }
        }
        else
        {
            setting.Value = value;
            repository.Update(setting);
            cacheManager.RemoveByPattern(string.Format(MantleWebConstants.CacheKeys.SettingsKeysPatternFormat, tenantId));
        }
    }

    public void SaveSettings<TSettings>(TSettings settings, int? tenantId = null) where TSettings : ISettings
    {
        var type = settings.GetType();
        string key = type.FullName;
        string value = settings.JsonSerialize();
        SaveSettings(key, value, tenantId);
    }
}