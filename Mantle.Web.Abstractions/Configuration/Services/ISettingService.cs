namespace Mantle.Web.Configuration.Services;

public interface ISettingService
{
    TSettings GetSettings<TSettings>(int? tenantId = null) where TSettings : ISettings, new();

    ISettings GetSettings(Type settingsType, int? tenantId = null);

    void SaveSettings(string key, string value, int? tenantId = null);

    void SaveSettings<TSettings>(TSettings settings, int? tenantId = null) where TSettings : ISettings;
}