using Mantle.Web.ContentManagement.Areas.Admin.Pages.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;
using Microsoft.AspNetCore.OData.Query.Validator;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages.Controllers.Api;

public class PageTreeApiController : ODataController
{
    private readonly IPageService service;
    private readonly IWorkContext workContext;

    public PageTreeApiController(IPageService service, IWorkContext workContext)
    {
        this.service = service;
        this.workContext = workContext;
    }

    public async Task<IEnumerable<PageTreeItem>> Get(ODataQueryOptions<PageTreeItem> options)
    {
        if (!CheckPermission(CmsPermissions.PagesRead))
        {
            return Enumerable.Empty<PageTreeItem>();
        }

        int tenantId = GetTenantId();
        var pages = await service.FindAsync(new SearchOptions<Page>
        {
            Query = x => x.TenantId == tenantId
        });

        var hierarchy = pages
            .Where(x => x.ParentId == null)
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Name)
            .Select(x => new PageTreeItem
            {
                Id = x.Id,
                Title = x.Name,
                IsEnabled = x.IsEnabled,
                SubPages = GetSubPages(pages, x.Id).ToList()
            });

        var settings = new ODataValidationSettings
        {
            AllowedQueryOptions = AllowedQueryOptions.All,
            MaxExpansionDepth = 10
        };
        options.Validate(settings);

        var results = options.ApplyTo(hierarchy.AsQueryable());
        return (results as IQueryable<PageTreeItem>).ToHashSet();
    }

    [EnableQuery]
    public virtual async Task<SingleResult<PageTreeItem>> Get([FromODataUri] Guid key)
    {
        if (!CheckPermission(CmsPermissions.PagesRead))
        {
            return SingleResult.Create(Enumerable.Empty<PageTreeItem>().AsQueryable());
        }

        int tenantId = GetTenantId();
        var pages = await service.FindAsync(new SearchOptions<Page>
        {
            Query = x => x.TenantId == tenantId
        });
        var entity = pages.FirstOrDefault(x => x.Id == key);

        return SingleResult.Create(new[] { entity }.Select(x => new PageTreeItem
        {
            Id = x.Id,
            Title = x.Name,
            IsEnabled = x.IsEnabled,
            SubPages = GetSubPages(pages, x.Id).ToList()
        }).AsQueryable());
    }

    private static IEnumerable<PageTreeItem> GetSubPages(IEnumerable<Page> pages, Guid parentId) => pages
        .Where(x => x.ParentId == parentId)
        .OrderBy(x => x.Order)
        .ThenBy(x => x.Name)
        .Select(x => new PageTreeItem
        {
            Id = x.Id,
            Title = x.Name,
            IsEnabled = x.IsEnabled,
            SubPages = GetSubPages(pages, x.Id).ToList()
        });

    protected virtual bool CheckPermission(Permission permission)
    {
        var authorizationService = DependoResolver.Instance.Resolve<IMantleAuthorizationService>();
        return authorizationService.TryCheckAccess(permission, workContext.CurrentUser);
    }

    protected virtual int GetTenantId() => workContext.CurrentTenant.Id;
}

public class PageTreeItem
{
    public PageTreeItem()
    {
        SubPages = [];
    }

    public Guid Id { get; set; }

    public string Title { get; set; }

    public bool IsEnabled { get; set; }

    public List<PageTreeItem> SubPages { get; set; }
}