using System;
using System.Linq;
using Mantle.Caching;
using Mantle.Data;
using Mantle.Infrastructure;
using Mantle.Configuration.Domain;

namespace Mantle.Web.Configuration.Services
{
    public interface ISettingService
    {
        TSettings GetSettings<TSettings>(int? tenantId = null) where TSettings : ISettings, new();

        ISettings GetSettings(Type settingsType, int? tenantId = null);

        void SaveSettings(string key, string value, int? tenantId = null);

        void SaveSettings<TSettings>(TSettings settings, int? tenantId = null) where TSettings : ISettings;
    }

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
                Setting settings = null;

                if (tenantId.HasValue)
                {
                    settings = repository.FindOne(x => x.TenantId == tenantId && x.Type == type);
                }
                else
                {
                    settings = repository.FindOne(x => x.TenantId == null && x.Type == type);
                }

                if (settings == null || string.IsNullOrEmpty(settings.Value))
                {
                    return new TSettings();
                }

                return settings.Value.JsonDeserialize<TSettings>();
            });
        }

        public ISettings GetSettings(Type settingsType, int? tenantId = null)
        {
            string type = settingsType.FullName;
            string key = string.Format(MantleWebConstants.CacheKeys.SettingsKeyFormat, tenantId, type);
            return cacheManager.Get(key, () =>
            {
                Setting settings = null;

                if (tenantId.HasValue)
                {
                    settings = repository.FindOne(x => x.TenantId == tenantId && x.Type == type);
                }
                else
                {
                    settings = repository.FindOne(x => x.TenantId == null && x.Type == type);
                }

                if (settings == null || string.IsNullOrEmpty(settings.Value))
                {
                    return (ISettings)Activator.CreateInstance(settingsType);
                }

                return (ISettings)settings.Value.JsonDeserialize(settingsType);
            });
        }

        public void SaveSettings(string key, string value, int? tenantId = null)
        {
            Setting setting = null;

            if (tenantId.HasValue)
            {
                setting = repository.FindOne(x => x.TenantId == tenantId && x.Type == key);
            }
            else
            {
                setting = repository.FindOne(x => x.TenantId == null && x.Type == key);
            }

            if (setting == null)
            {
                var iSettings = EngineContext.Current.ResolveAll<ISettings>().FirstOrDefault(x => x.GetType().FullName == key);

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
            var key = type.FullName;
            var value = settings.ToJson();
            SaveSettings(key, value, tenantId);
        }
    }
}