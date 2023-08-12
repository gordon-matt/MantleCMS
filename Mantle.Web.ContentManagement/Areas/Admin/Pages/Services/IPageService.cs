using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Entities;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;

public interface IPageService : IGenericDataService<Page>
{
    IEnumerable<Page> GetTopLevelPages(int tenantId);
}

public class PageService : GenericDataService<Page>, IPageService
{
    private readonly Lazy<IRepository<PageVersion>> pageVersionRepository;
    private readonly Lazy<IRepository<ContentBlock>> contentBlockRepository;
    private readonly Lazy<IRepository<SitemapConfig>> sitemapConfigRepository;

    public PageService(
        ICacheManager cacheManager,
        IRepository<Page> repository,
        Lazy<IRepository<PageVersion>> pageVersionRepository,
        Lazy<IRepository<ContentBlock>> contentBlockRepository,
        Lazy<IRepository<SitemapConfig>> sitemapConfigRepository)
        : base(cacheManager, repository)
    {
        this.pageVersionRepository = pageVersionRepository;
        this.contentBlockRepository = contentBlockRepository;
        this.sitemapConfigRepository = sitemapConfigRepository;
    }

    #region Delete

    public override int Delete(IEnumerable<Page> entities)
    {
        var pageIds = entities.Select(x => x.Id);

        // Delete Content Blocks
        int rowsAffected = contentBlockRepository.Value.Delete(x => x.PageId != null && pageIds.Contains(x.PageId.Value));

        // Delete Site Map Config
        rowsAffected += sitemapConfigRepository.Value.Delete(x => pageIds.Contains(x.PageId));

        // Delete Page Versions
        rowsAffected += pageVersionRepository.Value.Delete(x => pageIds.Contains(x.PageId));
        rowsAffected += base.Delete(entities);

        // Ensure No Orphans
        var pages = Find(x => pageIds.Contains(x.Id));
        EnsureNoOrphans(pages);

        ClearCache();
        return rowsAffected;
    }

    public override int Delete(IQueryable<Page> query)
    {
        var pageIds = query.Select(x => x.Id).ToArray();

        // Delete Content Blocks
        int rowsAffected = contentBlockRepository.Value.Delete(x => x.PageId != null && pageIds.Contains(x.PageId.Value));

        // Delete Site Map Config
        rowsAffected += sitemapConfigRepository.Value.Delete(x => pageIds.Contains(x.PageId));

        // Delete Page Versions
        rowsAffected += pageVersionRepository.Value.Delete(x => pageIds.Contains(x.PageId));
        rowsAffected += base.Delete(query);

        // Ensure No Orphans
        var pages = Find(x => pageIds.Contains(x.Id));
        EnsureNoOrphans(pages);

        ClearCache();
        return rowsAffected;
    }

    public override int Delete(Page entity)
    {
        // Delete Content Blocks
        int rowsAffected = contentBlockRepository.Value.Delete(x => x.PageId == entity.Id);

        // Delete Site Map Config
        rowsAffected += sitemapConfigRepository.Value.Delete(x => x.PageId == entity.Id);

        // Delete Page Versions
        rowsAffected += pageVersionRepository.Value.Delete(x => x.PageId == entity.Id);
        rowsAffected += base.Delete(entity);

        // Ensure No Orphans
        EnsureNoOrphans(entity);

        ClearCache();
        return rowsAffected;
    }

    public override int Delete(Expression<Func<Page, bool>> filterExpression)
    {
        var pages = Find(filterExpression);
        var pageIds = pages.Select(x => x.Id).ToArray();

        // Delete Content Blocks
        int rowsAffected = contentBlockRepository.Value.Delete(x => x.PageId != null && pageIds.Contains(x.PageId.Value));

        // Delete Site Map Config
        rowsAffected += sitemapConfigRepository.Value.Delete(x => pageIds.Contains(x.PageId));

        // Delete Page Versions
        rowsAffected += pageVersionRepository.Value.Delete(x => pageIds.Contains(x.PageId));
        rowsAffected += base.Delete(filterExpression);

        // Ensure No Orphans
        EnsureNoOrphans(pages);

        ClearCache();
        return rowsAffected;
    }

    public override int DeleteAll()
    {
        var pages = Find();
        var pageIds = pages.Select(x => x.Id).ToArray();

        // Delete Content Blocks
        int rowsAffected = contentBlockRepository.Value.Delete(x => x.PageId != null && pageIds.Contains(x.PageId.Value));

        // Delete Site Map Config
        rowsAffected += sitemapConfigRepository.Value.Delete(x => pageIds.Contains(x.PageId));

        // Delete Page Versions
        rowsAffected += pageVersionRepository.Value.Delete(x => pageIds.Contains(x.PageId));
        rowsAffected += base.DeleteAll();

        // Ensure No Orphans
        EnsureNoOrphans(pages);

        ClearCache();
        return rowsAffected;
    }

    public override async Task<int> DeleteAllAsync()
    {
        var pages = await FindAsync();
        var pageIds = pages.Select(x => x.Id).ToArray();

        // Delete Content Blocks
        int rowsAffected = await contentBlockRepository.Value.DeleteAsync(x => x.PageId != null && pageIds.Contains(x.PageId.Value));

        // Delete Site Map Config
        rowsAffected += await sitemapConfigRepository.Value.DeleteAsync(x => pageIds.Contains(x.PageId));

        // Delete Page Versions
        rowsAffected += await pageVersionRepository.Value.DeleteAsync(x => pageIds.Contains(x.PageId));
        rowsAffected += await base.DeleteAllAsync();

        // Ensure No Orphans
        await EnsureNoOrphansAsync(pages);

        ClearCache();
        return rowsAffected;
    }

    public override async Task<int> DeleteAsync(IEnumerable<Page> entities)
    {
        var pageIds = entities.Select(x => x.Id);

        // Delete Content Blocks
        int rowsAffected = await contentBlockRepository.Value.DeleteAsync(x => x.PageId != null && pageIds.Contains(x.PageId.Value));

        // Delete Site Map Config
        rowsAffected += await sitemapConfigRepository.Value.DeleteAsync(x => pageIds.Contains(x.PageId));

        // Delete Page Versions
        rowsAffected += await pageVersionRepository.Value.DeleteAsync(x => pageIds.Contains(x.PageId));
        rowsAffected += await base.DeleteAsync(entities);

        // Ensure No Orphans
        var pages = await FindAsync(x => pageIds.Contains(x.Id));
        await EnsureNoOrphansAsync(pages);

        ClearCache();
        return rowsAffected;
    }

    public override async Task<int> DeleteAsync(IQueryable<Page> query)
    {
        var pageIds = query.Select(x => x.Id).ToArray();

        // Delete Content Blocks
        int rowsAffected = await contentBlockRepository.Value.DeleteAsync(x => x.PageId != null && pageIds.Contains(x.PageId.Value));

        // Delete Site Map Config
        rowsAffected += await sitemapConfigRepository.Value.DeleteAsync(x => pageIds.Contains(x.PageId));

        // Delete Page Versions
        rowsAffected += await pageVersionRepository.Value.DeleteAsync(x => pageIds.Contains(x.PageId));
        rowsAffected += await base.DeleteAsync(query);

        // Ensure No Orphans
        var pages = await FindAsync(x => pageIds.Contains(x.Id));
        await EnsureNoOrphansAsync(pages);

        ClearCache();
        return rowsAffected;
    }

    public override async Task<int> DeleteAsync(Page entity)
    {
        // Delete Content Blocks
        int rowsAffected = await contentBlockRepository.Value.DeleteAsync(x => x.PageId == entity.Id);

        // Delete Site Map Config
        rowsAffected += await sitemapConfigRepository.Value.DeleteAsync(x => x.PageId == entity.Id);

        // Delete Page Versions
        rowsAffected += await pageVersionRepository.Value.DeleteAsync(x => x.PageId == entity.Id);
        rowsAffected += await base.DeleteAsync(entity);

        // Ensure No Orphans
        await EnsureNoOrphansAsync(entity);

        ClearCache();
        return rowsAffected;
    }

    public override async Task<int> DeleteAsync(Expression<Func<Page, bool>> filterExpression)
    {
        var pages = await FindAsync(filterExpression);
        var pageIds = pages.Select(x => x.Id).ToArray();

        // Delete Content Blocks
        int rowsAffected = await contentBlockRepository.Value.DeleteAsync(x => x.PageId != null && pageIds.Contains(x.PageId.Value));

        // Delete Site Map Config
        rowsAffected += await sitemapConfigRepository.Value.DeleteAsync(x => pageIds.Contains(x.PageId));

        // Delete Page Versions
        rowsAffected += await pageVersionRepository.Value.DeleteAsync(x => pageIds.Contains(x.PageId));
        rowsAffected += await base.DeleteAsync(filterExpression);

        // Ensure No Orphans
        await EnsureNoOrphansAsync(pages);

        ClearCache();
        return rowsAffected;
    }

    #endregion Delete

    #region Insert

    public override int Insert(IEnumerable<Page> entities)
    {
        int rowsAffected = base.Insert(entities);

        var pageVersions = entities.Select(x => new PageVersion
        {
            Id = Guid.NewGuid(),
            TenantId = x.TenantId,
            PageId = x.Id,
            CultureCode = null,
            DateCreatedUtc = DateTime.UtcNow,
            DateModifiedUtc = DateTime.UtcNow,
            Status = VersionStatus.Draft,
            Title = x.Name,
            Slug = x.Name.ToSlugUrl()
        });
        rowsAffected += pageVersionRepository.Value.Insert(pageVersions);

        ClearCache();
        return rowsAffected;
    }

    public override int Insert(Page entity)
    {
        int rowsAffected = base.Insert(entity);

        rowsAffected += pageVersionRepository.Value.Insert(new PageVersion
        {
            Id = Guid.NewGuid(),
            TenantId = entity.TenantId,
            PageId = entity.Id,
            CultureCode = null,
            DateCreatedUtc = DateTime.UtcNow,
            DateModifiedUtc = DateTime.UtcNow,
            Status = VersionStatus.Draft,
            Title = entity.Name,
            Slug = entity.Name.ToSlugUrl()
        });

        ClearCache();
        return rowsAffected;
    }

    public override async Task<int> InsertAsync(IEnumerable<Page> entities)
    {
        int rowsAffected = await base.InsertAsync(entities);

        var pageVersions = entities.Select(x => new PageVersion
        {
            Id = Guid.NewGuid(),
            TenantId = x.TenantId,
            PageId = x.Id,
            CultureCode = null,
            DateCreatedUtc = DateTime.UtcNow,
            DateModifiedUtc = DateTime.UtcNow,
            Status = VersionStatus.Draft,
            Title = x.Name,
            Slug = x.Name.ToSlugUrl()
        });
        rowsAffected += await pageVersionRepository.Value.InsertAsync(pageVersions);

        ClearCache();
        return rowsAffected;
    }

    public override async Task<int> InsertAsync(Page entity)
    {
        int rowsAffected = await base.InsertAsync(entity);

        rowsAffected += await pageVersionRepository.Value.InsertAsync(new PageVersion
        {
            Id = Guid.NewGuid(),
            TenantId = entity.TenantId,
            PageId = entity.Id,
            CultureCode = null,
            DateCreatedUtc = DateTime.UtcNow,
            DateModifiedUtc = DateTime.UtcNow,
            Status = VersionStatus.Draft,
            Title = entity.Name,
            Slug = entity.Name.ToSlugUrl()
        });

        ClearCache();
        return rowsAffected;
    }

    #endregion Insert

    private void EnsureNoOrphans(Page page)
    {
        var toUpdate = new List<Page>();

        var subPages = Find(x => x.ParentId == page.Id);
        subPages.ForEach(x =>
        {
            x.ParentId = page.ParentId;
            toUpdate.Add(x);
        });

        Update(toUpdate);
    }

    private void EnsureNoOrphans(IEnumerable<Page> pages)
    {
        var toUpdate = new List<Page>();
        foreach (var page in pages)
        {
            var subPages = Find(x => x.ParentId == page.Id);

            subPages.ForEach(x =>
            {
                x.ParentId = page.ParentId;
                toUpdate.Add(x);
            });
        }
        Update(toUpdate);
    }

    private async Task EnsureNoOrphansAsync(Page page)
    {
        var toUpdate = new List<Page>();

        var subPages = await FindAsync(x => x.ParentId == page.Id);
        subPages.ForEach(x =>
        {
            x.ParentId = page.ParentId;
            toUpdate.Add(x);
        });

        await UpdateAsync(toUpdate);
    }

    private async Task EnsureNoOrphansAsync(IEnumerable<Page> pages)
    {
        var toUpdate = new List<Page>();
        foreach (var page in pages)
        {
            var subPages = await FindAsync(x => x.ParentId == page.Id);

            subPages.ForEach(x =>
            {
                x.ParentId = page.ParentId;
                toUpdate.Add(x);
            });
        }
        await UpdateAsync(toUpdate);
    }

    #region IPageService Members

    public IEnumerable<Page> GetTopLevelPages(int tenantId)
    {
        using var connection = OpenConnection();
        return connection
            .Query(x => x.TenantId == tenantId && x.ParentId == null)
            .OrderBy(x => x.Name)
            .ToHashSet();
    }

    #endregion IPageService Members
}