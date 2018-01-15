using System;
using System.Collections.Generic;
using Mantle.Web.Configuration;
using Mantle.Web.Indexing.Configuration;
using Mantle.Web.IO.FileSystems.AppData;
using Mantle.Web.IO.FileSystems.LockFile;

namespace Mantle.Web.Indexing.Services
{
    public class IndexingTaskExecutor : IIndexingTaskExecutor, IIndexStatisticsProvider
    {
        private IIndexProvider indexProvider;
        private IndexingStatus indexingStatus = IndexingStatus.Idle;
        private readonly IAppDataFolder appDataFolder;
        private readonly IEnumerable<IIndexingContentProvider> contentProviders;
        private readonly IIndexManager indexManager;
        private readonly ILockFileManager lockFileManager;
        private readonly SiteSettings siteSettings;
        private readonly Lazy<ILogger> logger;

        public IndexingTaskExecutor(
            IAppDataFolder appDataFolder,
            IEnumerable<IIndexingContentProvider> contentProviders,
            IIndexManager indexManager,
            ILockFileManager lockFileManager,
            SiteSettings siteSettings,
            Lazy<ILogger> logger)
        {
            this.appDataFolder = appDataFolder;
            this.contentProviders = contentProviders;
            this.indexManager = indexManager;
            this.lockFileManager = lockFileManager;
            this.logger = logger;
            this.siteSettings = siteSettings;
        }

        public bool DeleteIndex(string indexName)
        {
            ILockFile lockFile = null;
            var settingsFilename = GetSettingsFileName(indexName);
            var lockFilename = settingsFilename + ".lock";

            // acquire a lock file on the index
            if (!lockFileManager.TryAcquireLock(lockFilename, ref lockFile))
            {
                logger.Value.Info("Could not delete the index. Already in use.");
                return false;
            }

            using (lockFile)
            {
                if (!indexManager.HasIndexProvider())
                {
                    return false;
                }

                var searchProvider = indexManager.GetSearchIndexProvider();
                if (searchProvider.Exists(indexName))
                {
                    searchProvider.DeleteIndex(indexName);
                }

                DeleteSettings(indexName);
            }

            return true;
        }

        public bool UpdateIndex(string indexName)
        {
            ILockFile lockFile = null;
            var settingsFilename = GetSettingsFileName(indexName);
            var lockFilename = settingsFilename + ".lock";

            // acquire a lock file on the index
            if (!lockFileManager.TryAcquireLock(lockFilename, ref lockFile))
            {
                logger.Value.Info("Index was requested but is already running");
                return false;
            }

            using (lockFile)
            {
                if (!indexManager.HasIndexProvider())
                {
                    return false;
                }

                // load index settings to know what is the current state of indexing
                var indexSettings = LoadSettings(indexName);

                indexProvider = indexManager.GetSearchIndexProvider();
                indexProvider.CreateIndex(indexName);

                return UpdateIndex(indexName, settingsFilename, indexSettings);
            }
        }

        private bool UpdateIndex(string indexName, string settingsFilename, IndexSettings indexSettings)
        {
            var addToIndex = new List<IDocumentIndex>();

            // Rebuilding the inde
            logger.Value.Info("Rebuilding index");
            indexingStatus = IndexingStatus.Rebuilding;

            foreach (var contentProvider in contentProviders)
            {
                addToIndex.AddRange(contentProvider.GetDocuments(id => indexProvider.New(id)));
            }

            // save current state of the index
            indexSettings.LastIndexedUtc = DateTime.UtcNow;
            appDataFolder.CreateFile(settingsFilename, indexSettings.ToXml());

            if (addToIndex.Count == 0)
            {
                // nothing more to do
                indexingStatus = IndexingStatus.Idle;
                return false;
            }

            // save new and updated documents to the index
            indexProvider.Store(indexName, addToIndex);
            logger.Value.InfoFormat("Added documents to index: {0}", addToIndex.Count);

            return true;
        }

        public DateTime GetLastIndexedUtc(string indexName)
        {
            var indexSettings = LoadSettings(indexName);
            return indexSettings.LastIndexedUtc;
        }

        public IndexingStatus GetIndexingStatus(string indexName)
        {
            return indexingStatus;
        }

        private string GetSettingsFileName(string indexName)
        {
            return appDataFolder.Combine("Sites", siteSettings.SiteName, indexName + ".settings.xml");
        }

        /// <summary>
        /// Loads the settings file or create a new default one if it doesn't exist
        /// </summary>
        private IndexSettings LoadSettings(string indexName)
        {
            var indexSettings = new IndexSettings();
            var settingsFilename = GetSettingsFileName(indexName);
            if (appDataFolder.FileExists(settingsFilename))
            {
                var content = appDataFolder.ReadFile(settingsFilename);
                indexSettings = IndexSettings.Parse(content);
            }

            return indexSettings;
        }

        /// <summary>
        /// Deletes the settings file
        /// </summary>
        private void DeleteSettings(string indexName)
        {
            var settingsFilename = GetSettingsFileName(indexName);
            if (appDataFolder.FileExists(settingsFilename))
            {
                appDataFolder.DeleteFile(settingsFilename);
            }
        }
    }
}