using Mantle.Events;

namespace Mantle.Web.Indexing
{
    public interface IIndexNotifierHandler : IEventHandler
    {
        void UpdateIndex(string indexName);
    }
}