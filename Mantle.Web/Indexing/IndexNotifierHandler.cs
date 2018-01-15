using Mantle.Web.Indexing.Services;

namespace Mantle.Web.Indexing
{
    public class IndexNotifierHandler : IIndexNotifierHandler
    {
        private readonly IIndexingTaskExecutor indexingTaskExecutor;

        public IndexNotifierHandler(IIndexingTaskExecutor indexingTaskExecutor)
        {
            this.indexingTaskExecutor = indexingTaskExecutor;
        }

        public void UpdateIndex(string indexName)
        {
            indexingTaskExecutor.UpdateIndex(indexName);
        }
    }
}