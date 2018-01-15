namespace Mantle.Web.Indexing.Services
{
    public interface IIndexingTaskExecutor
    {
        bool DeleteIndex(string indexName);

        bool UpdateIndex(string indexName);
    }
}