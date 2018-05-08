using System;
using System.Collections.Generic;
using System.Linq;
using Mantle.Caching;
using Mantle.Data;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Localization.Domain;

namespace Mantle.Plugins
{
    /// <summary>
    /// Base plugin
    /// </summary>
    public abstract class BasePlugin : IPlugin
    {
        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public virtual string GetConfigurationPageUrl()
        {
            return null;
        }

        /// <summary>
        /// Gets or sets the plugin descriptor
        /// </summary>
        public virtual PluginDescriptor PluginDescriptor { get; set; }

        /// <summary>
        /// Install plugin
        /// </summary>
        public virtual void Install()
        {
            PluginManager.MarkPluginAsInstalled(this.PluginDescriptor.SystemName);
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public virtual void Uninstall()
        {
            PluginManager.MarkPluginAsUninstalled(this.PluginDescriptor.SystemName);
        }

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

            var localizableStringRepository = EngineContext.Current.Resolve<IRepository<LocalizableString>>();
            localizableStringRepository.Insert(toInsert);

            var cacheManager = EngineContext.Current.Resolve<ICacheManager>();
            cacheManager.RemoveByPattern(CacheKeys.LocalizableStringsPatternFormat);
        }

        protected virtual void UninstallLanguagePack<TLanguagePack>() where TLanguagePack : ILanguagePack, new()
        {
            var languagePack = new TLanguagePack();

            var distinctKeys = languagePack
                .LocalizedStrings.Select(y => y.Key)
                .Distinct();

            var localizableStringRepository = EngineContext.Current.Resolve<IRepository<LocalizableString>>();

            var toDelete = localizableStringRepository.Find(x => distinctKeys.Contains(x.TextKey));

            localizableStringRepository.Delete(toDelete);

            var cacheManager = EngineContext.Current.Resolve<ICacheManager>();
            cacheManager.RemoveByPattern(CacheKeys.LocalizableStringsPatternFormat);
        }
    }
}