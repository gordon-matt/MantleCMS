using System;
using System.Collections.Generic;

namespace Mantle.Web.Indexing.Services
{
    public class IndexEntry
    {
        public string IndexName { get; set; }

        public int DocumentCount { get; set; }

        public DateTime LastUpdateUtc { get; set; }

        public IEnumerable<string> Fields { get; set; }

        public IndexingStatus IndexingStatus { get; set; }
    }
}