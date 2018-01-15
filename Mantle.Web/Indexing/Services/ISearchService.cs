using System.Globalization;
using System.Linq;
using Mantle.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.Indexing.Services
{
    public interface ISearchService
    {
        PagedList<ISearchHit> Query(string query, string cultureCode, int pageIndex = 0, int? pageSize = null);
    }

    public class SearchService : ISearchService
    {
        private readonly IIndexManager indexManager;

        public SearchService(IIndexManager indexManager, IStringLocalizer localizer)
        {
            this.indexManager = indexManager;
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        private ISearchBuilder Search()
        {
            return indexManager.HasIndexProvider()
                ? indexManager.GetSearchIndexProvider().CreateSearchBuilder("Search")
                : new NullSearchBuilder();
        }

        public PagedList<ISearchHit> Query(string query, string cultureCode, int pageIndex = 0, int? pageSize = null)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new PagedList<ISearchHit>(Enumerable.Empty<ISearchHit>());
            }

            var provider = indexManager.GetSearchIndexProvider();
            var searchFields = provider.GetFields(MantleWebConstants.Indexing.DefaultIndexName)
                .Except(new[] { "id", "culture", "url", "description" })
                .ToArray();

            var searchBuilder = Search().Parse(searchFields, query);

            if (!string.IsNullOrEmpty(cultureCode))
            {
                var cultureInfo = new CultureInfo(cultureCode);
                searchBuilder.WithField("culture", cultureInfo.LCID).AsFilter();
            }
            else
            {
                searchBuilder.WithField("culture", CultureInfo.InvariantCulture.LCID).AsFilter();
            }

            if (MantleWebConfigurationSection.Instance.Indexing.ExactMatch)
            {
                searchBuilder.ExactMatch();
            }

            int totalCount = searchBuilder.Count();

            if (pageSize.HasValue)
            {
                searchBuilder = searchBuilder.Slice(
                    (pageIndex > 0 ? pageIndex - 1 : 0) * pageSize.Value,
                    pageSize.Value);
            }

            var searchResults = searchBuilder.Search();

            return new PagedList<ISearchHit>(searchResults, pageIndex, pageSize ?? totalCount, totalCount);
        }
    }
}