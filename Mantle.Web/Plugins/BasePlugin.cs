using System;
using System.Collections.Generic;
using System.Linq;
using Mantle.Caching;
using Mantle.Data;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Localization.Domain;

namespace Mantle.Web.Plugins
{
    public abstract class BasePlugin : IPlugin
    {
        protected BasePlugin()
        {
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

        /// <summary>
        /// Notify the plugin that settings have been modified (generally speaking,
        /// we're only interested in the "LimitedToTenants" property
        /// </summary>
        public virtual void Modified()
        {
            // By default, do nothing (very few plugins care)
        }

        //protected bool CheckIfTableExists(DbContext dbContext, string tableName)
        //{
        //    return dbContext.Database
        //        .SqlQuery<int?>(string.Format("SELECT 1 FROM sys.tables WHERE Name = '{0}'", tableName))
        //        .SingleOrDefault() != null;
        //}

        //protected void DropTable(DbContext dbContext, string tableName)
        //{
        //    if (CheckIfTableExists(dbContext, tableName))
        //    {
        //        dbContext.Database.ExecuteSqlCommand(string.Format("DROP TABLE [dbo].[{0}]", tableName));
        //    }
        //}

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
            cacheManager.RemoveByPattern(MantleConstants.CacheKeys.LocalizableStringsPatternFormat);
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
            cacheManager.RemoveByPattern(MantleConstants.CacheKeys.LocalizableStringsPatternFormat);
        }
    }
}