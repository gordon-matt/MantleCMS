using Mantle.Localization;
using Mantle.Localization.Entities;
using Mantle.Web;

namespace Mantle.Plugins;

/// <summary>
/// Base plugin
/// </summary>
public abstract class BasePlugin : IPlugin
{
    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public virtual string GetConfigurationPageUrl() => null;

    /// <summary>
    /// Gets or sets the plugin descriptor
    /// </summary>
    public virtual PluginDescriptor PluginDescriptor { get; set; }

    /// <summary>
    /// Install plugin
    /// </summary>
    public virtual void Install() => PluginManager.MarkPluginAsInstalled(this.PluginDescriptor.SystemName);

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    public virtual void Uninstall() => PluginManager.MarkPluginAsUninstalled(this.PluginDescriptor.SystemName);

    protected virtual void InstallLanguagePack<TLanguagePack>() where TLanguagePack : ILanguagePack, new()
    {
        var toInsert = new HashSet<LocalizableString>();

        var languagePack = new TLanguagePack();
        foreach (var localizedString in languagePack.LocalizedStrings)
        {
            toInsert.Add(new LocalizableString
            {
                Id = Guid.NewGuid(),
                CultureCode = languagePack.CultureCode,
                TextKey = localizedString.Key,
                TextValue = localizedString.Value
            });
        }

        var localizableStringRepository = DependoResolver.Instance.Resolve<IRepository<LocalizableString>>();
        localizableStringRepository.Insert(toInsert);

        var workContext = DependoResolver.Instance.Resolve<IWorkContext>();
        string cacheKey = string.Format(CacheKeys.LocalizableStringsPatternFormat, workContext.CurrentTenant.Id);

        var cacheManager = DependoResolver.Instance.Resolve<ICacheManager>();
        cacheManager.RemoveByPattern(cacheKey);
    }

    protected virtual void UninstallLanguagePack<TLanguagePack>() where TLanguagePack : ILanguagePack, new()
    {
        var languagePack = new TLanguagePack();

        var distinctKeys = languagePack
            .LocalizedStrings.Select(y => y.Key)
            .Distinct();

        var localizableStringRepository = DependoResolver.Instance.Resolve<IRepository<LocalizableString>>();

        var toDelete = localizableStringRepository.Find(new SearchOptions<LocalizableString>
        {
            Query = x => distinctKeys.Contains(x.TextKey)
        });

        localizableStringRepository.Delete(toDelete);

        var workContext = DependoResolver.Instance.Resolve<IWorkContext>();
        string cacheKey = string.Format(CacheKeys.LocalizableStringsPatternFormat, workContext.CurrentTenant.Id);

        var cacheManager = DependoResolver.Instance.Resolve<ICacheManager>();
        cacheManager.RemoveByPattern(cacheKey);
    }
}