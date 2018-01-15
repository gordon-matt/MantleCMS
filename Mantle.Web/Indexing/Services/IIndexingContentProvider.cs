using System;
using System.Collections.Generic;

namespace Mantle.Web.Indexing.Services
{
    public interface IIndexingContentProvider
    {
        IEnumerable<IDocumentIndex> GetDocuments(Func<string, IDocumentIndex> factory);
    }
}