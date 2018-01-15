using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Web.Mvc.Notify;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.Indexing.Services
{
    public class IndexingService : IIndexingService
    {
        private readonly IIndexManager indexManager;
        private readonly IEnumerable<IIndexNotifierHandler> indexNotifierHandlers;
        private readonly IIndexStatisticsProvider indexStatisticsProvider;
        private readonly IIndexingTaskExecutor indexingTaskExecutor;
        private readonly INotifier notifier;

        public IndexingService(
            IIndexManager indexManager,
            IEnumerable<IIndexNotifierHandler> indexNotifierHandlers,
            IIndexStatisticsProvider indexStatisticsProvider,
            IIndexingTaskExecutor indexingTaskExecutor,
            INotifier notifier)
        {
            this.indexManager = indexManager;
            this.indexNotifierHandlers = indexNotifierHandlers;
            this.indexStatisticsProvider = indexStatisticsProvider;
            this.indexingTaskExecutor = indexingTaskExecutor;
            this.notifier = notifier;
            T = EngineContext.Current.Resolve<IStringLocalizer>();
        }

        public IStringLocalizer T { get; set; }

        public void RebuildIndex(string indexName)
        {
            if (!indexManager.HasIndexProvider())
            {
                notifier.Warning(T[MantleWebLocalizableStrings.Indexing.NoSearchIndexToRebuild].Value);
                return;
            }

            if (indexingTaskExecutor.DeleteIndex(indexName))
            {
                notifier.Info(string.Format(T[MantleWebLocalizableStrings.Indexing.SearchIndexRebuilt].Value, indexName));
                UpdateIndex(indexName);
            }
            else
            {
                notifier.Warning(string.Format(T[MantleWebLocalizableStrings.Indexing.SearchIndexRebuildFail].Value, indexName));
            }
        }

        public void UpdateIndex(string indexName)
        {
            foreach (var handler in indexNotifierHandlers)
            {
                handler.UpdateIndex(indexName);
            }

            notifier.Info(T[MantleWebLocalizableStrings.Indexing.SearchIndexUpdated].Value);
        }

        IndexEntry IIndexingService.GetIndexEntry(string indexName)
        {
            var provider = indexManager.GetSearchIndexProvider();
            if (provider == null)
                return null;

            return new IndexEntry
            {
                IndexName = indexName,
                DocumentCount = provider.NumDocs(indexName),
                Fields = provider.GetFields(indexName),
                LastUpdateUtc = indexStatisticsProvider.GetLastIndexedUtc(indexName),
                IndexingStatus = indexStatisticsProvider.GetIndexingStatus(indexName)
            };
        }
    }
}