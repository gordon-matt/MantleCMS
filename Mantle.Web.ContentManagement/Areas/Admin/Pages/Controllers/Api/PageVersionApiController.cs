using Mantle.Web.ContentManagement.Areas.Admin.Media;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages.Controllers.Api;

public class PageVersionApiController : GenericTenantODataController<PageVersion, Guid>
{
    private readonly Lazy<IPageService> pageService;
    private readonly PageSettings settings;

    public PageVersionApiController(
        IPageVersionService service,
        Lazy<IPageService> pageService,
        PageSettings settings)
        : base(service)
    {
        this.pageService = pageService;
        this.settings = settings;
    }

    public override async Task<IActionResult> Delete([FromODataUri] Guid key)
    {
        var entity = await Service.FindOneAsync(key);

        if (entity == null)
        {
            return NotFound();
        }

        if (!CanModifyEntity(entity))
        {
            return Unauthorized();
        }

        // First find previous version and set it to be the current
        PageVersion previous = null;
        using (var connection = Service.OpenConnection())
        {
            previous = await connection
                .Query(x =>
                    x.Id != entity.Id &&
                    x.PageId == entity.PageId &&
                    x.CultureCode == entity.CultureCode)
                .OrderByDescending(x => x.DateModifiedUtc)
                .FirstOrDefaultAsync();
        }

        if (previous == null)
        {
            var localizer = EngineContext.Current.Resolve<IStringLocalizer>();
            return BadRequest(localizer[MantleCmsLocalizableStrings.Pages.CannotDeleteOnlyVersion].Value);
        }

        previous.Status = VersionStatus.Published;
        await Service.UpdateAsync(previous);

        return await base.Delete(key);
    }

    [AcceptVerbs("PATCH", "MERGE")]
    public override async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<PageVersion> patch)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = await Service.FindOneAsync(key);

        if (entity == null)
        {
            return NotFound();
        }

        if (!CanModifyEntity(entity))
        {
            return Unauthorized();
        }

        patch.Patch(entity);

        try
        {
            //TODO: might have a big bug here:
            // shouldn't we be gettig BY culture code?
            var currentVersion = await Service.FindOneAsync(entity.Id);

            if (currentVersion.Status == VersionStatus.Published)
            {
                // archive current version before updating
                var backup = new PageVersion
                {
                    Id = Guid.NewGuid(),
                    TenantId = currentVersion.TenantId,
                    PageId = currentVersion.PageId,
                    CultureCode = currentVersion.CultureCode,
                    DateCreatedUtc = currentVersion.DateCreatedUtc,
                    DateModifiedUtc = currentVersion.DateModifiedUtc,
                    Status = VersionStatus.Archived,
                    Title = currentVersion.Title,
                    Slug = currentVersion.Slug,
                    Fields = currentVersion.Fields,
                };
                await Service.InsertAsync(backup);

                RemoveOldVersions(currentVersion.PageId, currentVersion.CultureCode);
            }

            entity.DateModifiedUtc = DateTime.UtcNow;
            await Service.UpdateAsync(entity);
        }
        catch (DbUpdateConcurrencyException x)
        {
            Logger.LogError(new EventId(), x, x.Message);

            if (!EntityExists(key))
            {
                return NotFound();
            }
            else { throw; }
        }

        return Updated(entity);
    }

    public override async Task<IActionResult> Post([FromBody] PageVersion entity)
    {
        entity.DateCreatedUtc = DateTime.UtcNow;
        entity.DateModifiedUtc = DateTime.UtcNow;
        entity.Fields = MediaHelper.EnsureCorrectUrls(entity.Fields);
        return await base.Post(entity);
    }

    public override async Task<IActionResult> Put([FromODataUri] Guid key, [FromBody] PageVersion entity)
    {
        if (!CanModifyEntity(entity))
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!key.Equals(GetId(entity)))
        {
            return BadRequest();
        }

        try
        {
            // Getting by ID only is not good enough in this instance, because GetCurrentVersion()
            //  will return the invariant record if no localized record exists. That means when an entity is updated to here,
            //  we are trying to update a localized one, but using the ID of the invariant one! So, we need to get by culture code AND ID
            //  and then if not exists, create it.
            var currentVersion = await Service.FindOneAsync(entity.Id);

            if (currentVersion.CultureCode != entity.CultureCode)
            {
                // We need to duplicate it to a new record!
                var newRecord = new PageVersion
                {
                    Id = Guid.NewGuid(),
                    TenantId = currentVersion.TenantId,
                    PageId = entity.PageId,
                    CultureCode = entity.CultureCode,
                    Status = currentVersion.Status,
                    Title = entity.Title,
                    Slug = entity.Slug,
                    Fields = MediaHelper.EnsureCorrectUrls(entity.Fields),
                    DateCreatedUtc = DateTime.UtcNow,
                    DateModifiedUtc = DateTime.UtcNow,
                };
                await Service.InsertAsync(newRecord);
            }
            else
            {
                if (currentVersion.Status == VersionStatus.Published)
                {
                    // archive current version before updating
                    var backup = new PageVersion
                    {
                        Id = Guid.NewGuid(),
                        TenantId = currentVersion.TenantId,
                        PageId = currentVersion.PageId,
                        CultureCode = currentVersion.CultureCode,
                        DateCreatedUtc = currentVersion.DateCreatedUtc,
                        DateModifiedUtc = currentVersion.DateModifiedUtc,
                        Status = VersionStatus.Archived,
                        Title = currentVersion.Title,
                        Slug = currentVersion.Slug,
                        Fields = currentVersion.Fields,
                    };
                    await Service.InsertAsync(backup);

                    RemoveOldVersions(currentVersion.PageId, currentVersion.CultureCode);
                }

                entity.TenantId = currentVersion.TenantId;
                entity.DateCreatedUtc = currentVersion.DateCreatedUtc;
                entity.DateModifiedUtc = DateTime.UtcNow;
                entity.Fields = MediaHelper.EnsureCorrectUrls(entity.Fields);
                await Service.UpdateAsync(entity);
            }
        }
        catch (DbUpdateConcurrencyException x)
        {
            Logger.LogError(new EventId(), x, x.Message);

            if (!EntityExists(key))
            {
                return NotFound();
            }
            else { throw; }
        }

        return Updated(entity);
    }

    protected override Guid GetId(PageVersion entity)
    {
        return entity.Id;
    }

    protected override void SetNewId(PageVersion entity)
    {
        entity.Id = Guid.NewGuid();
    }

    [HttpPost]
    public async Task<IActionResult> RestoreVersion([FromODataUri] Guid key, [FromBody] ODataActionParameters parameters)
    {
        if (!CheckPermission(CmsPermissions.PageHistoryRestore))
        {
            return Unauthorized();
        }

        var versionToRestore = await Service.FindOneAsync(key);

        if (versionToRestore == null)
        {
            return NotFound();
        }

        if (!CanModifyEntity(versionToRestore))
        {
            return Unauthorized();
        }

        int tenantId = GetTenantId();
        var current = ((IPageVersionService)Service).GetCurrentVersion(
            tenantId,
            versionToRestore.PageId,
            versionToRestore.CultureCode,
            enabledOnly: false,
            shownOnMenusOnly: false);

        if (current == null)
        {
            return NotFound();
        }

        // Archive the current one...
        var backup = new PageVersion
        {
            Id = Guid.NewGuid(),
            TenantId = current.TenantId,
            PageId = current.PageId,
            CultureCode = current.CultureCode,
            DateCreatedUtc = current.DateCreatedUtc,
            DateModifiedUtc = current.DateModifiedUtc,
            Status = VersionStatus.Archived,
            Title = current.Title,
            Slug = current.Slug,
            Fields = current.Fields,
        };
        await Service.InsertAsync(backup);

        RemoveOldVersions(current.PageId, current.CultureCode);

        // then restore the historical page, as requested
        current.CultureCode = versionToRestore.CultureCode;
        current.DateCreatedUtc = versionToRestore.DateCreatedUtc;
        current.DateModifiedUtc = versionToRestore.DateModifiedUtc;
        current.Status = VersionStatus.Published;
        current.Title = versionToRestore.Title;
        current.Slug = versionToRestore.Slug;
        current.Fields = versionToRestore.Fields;

        await Service.UpdateAsync(current);

        return Ok();
    }

    [HttpGet]
    public IActionResult GetCurrentVersion([FromODataUri] Guid pageId, [FromODataUri] string cultureCode)
    {
        if (!CheckPermission(CmsPermissions.PagesWrite))
        {
            return Unauthorized();
        }

        int tenantId = GetTenantId();
        var currentVersion = ((IPageVersionService)Service).GetCurrentVersion(
            tenantId,
            pageId,
            cultureCode,
            enabledOnly: false,
            shownOnMenusOnly: false);

        if (currentVersion == null)
        {
            return NotFound();
        }

        var pageVersion = new EdmPageVersion
        {
            Id = currentVersion.Id,
            PageId = currentVersion.PageId,
            CultureCode = currentVersion.CultureCode,
            Status = currentVersion.Status,
            Title = currentVersion.Title,
            Slug = currentVersion.Slug,
            Fields = currentVersion.Fields
        };

        return Ok(pageVersion);
    }

    protected override Permission ReadPermission
    {
        get { return CmsPermissions.PagesRead; }
    }

    protected override Permission WritePermission
    {
        get { return CmsPermissions.PagesWrite; }
    }

    private void RemoveOldVersions(Guid pageId, string cultureCode)
    {
        List<Guid> pageIdsToKeep = null;
        using (var connection = Service.OpenConnection())
        {
            pageIdsToKeep = connection
                .Query(x =>
                    x.PageId == pageId &&
                    x.CultureCode == cultureCode)
                .OrderByDescending(x => x.DateModifiedUtc)
                .Take(settings.NumberOfPageVersionsToKeep)
                .Select(x => x.Id)
                .ToList();
        }

        Service.Delete(x =>
            x.PageId == pageId &&
            x.CultureCode == cultureCode &&
            !pageIdsToKeep.Contains(x.Id));
    }
}

public struct EdmPageVersion
{
    public Guid Id { get; set; }

    public Guid PageId { get; set; }

    public string CultureCode { get; set; }

    public VersionStatus Status { get; set; }

    public string Title { get; set; }

    public string Slug { get; set; }

    public string Fields { get; set; }
}