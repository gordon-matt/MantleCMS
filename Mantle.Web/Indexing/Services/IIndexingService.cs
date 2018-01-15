namespace Mantle.Web.Indexing.Services
{
    public interface IIndexingService
    {
        void RebuildIndex(string indexName);

        void UpdateIndex(string indexName);

        IndexEntry GetIndexEntry(string indexName);
    }
}