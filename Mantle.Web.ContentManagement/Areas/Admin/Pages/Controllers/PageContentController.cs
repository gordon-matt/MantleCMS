using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;
using System.Text.RegularExpressions;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages.Controllers;

//[Area(CmsConstants.Areas.Pages)]
//[Route("admin/pages")]
public class PageContentController : MantleController
{
    protected static Regex ContentZonePattern = new(@"\[\[ContentZone:(?<Zone>.*)\]\]", RegexOptions.Compiled);
    private readonly IContentBlockService contentBlockService;
    private readonly Lazy<IMembershipService> membershipService;
    private readonly IPageService pageService;
    private readonly IPageVersionService pageVersionService;
    private readonly IPageTypeService pageTypeService;
    private readonly IRepository<Zone> zoneRepository;
    private readonly IRazorViewRenderService razorViewRenderService;

    public PageContentController(
        IPageService pageService,
        IPageVersionService pageVersionService,
        IPageTypeService pageTypeService,
        IContentBlockService contentBlockService,
        IRepository<Zone> zoneRepository,
        Lazy<IMembershipService> membershipService,
        IRazorViewRenderService razorViewRenderService)
        : base()
    {
        this.pageService = pageService;
        this.pageVersionService = pageVersionService;
        this.pageTypeService = pageTypeService;
        this.contentBlockService = contentBlockService;
        this.zoneRepository = zoneRepository;
        this.membershipService = membershipService;
        this.razorViewRenderService = razorViewRenderService;
    }

    //[OutputCache(Duration = 600, VaryByParam = "slug")] //TODO: Uncomment this when ready
    [Route("{slug}")]
    public async Task<ActionResult> Index(string slug)
    {
        //// Hack to make it search the correct path for the view
        //if (!this.ControllerContext.RouteData.DataTokens.ContainsKey("area"))
        //{
        //    this.ControllerContext.RouteData.DataTokens.Add("area", CmsConstants.Areas.Pages);
        //}

        int tenantId = WorkContext.CurrentTenant.Id;
        string currentCulture = WorkContext.CurrentCultureCode;

        //TODO: To support localized routes, we should probably first try get a single record by slug,
        //  then if there's only 1, fine.. return it.. if more than one.. then add cultureCode as
        //  we currently do...

        // First try get the latest published version for the current culture
        PageVersion pageVersion;
        using (var connection = pageVersionService.OpenConnection())
        {
            pageVersion = await connection.Query()
                .Include(x => x.Page)
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.Status == VersionStatus.Published &&
                    x.CultureCode == currentCulture &&
                    x.Slug == slug)
                .OrderByDescending(x => x.DateModifiedUtc)
                .FirstOrDefaultAsync();
        }

        // If there isn't one...
        if (pageVersion == null)
        {
            // ...then try get the last archived one for the current culture
            // NOTE: there's no need to worry about the last one being a draft before being archived, because
            //  we ONLY archive the published ones, not drafts.. so getting the last archived one will be the last published one
            using var connection = pageVersionService.OpenConnection();
            pageVersion = await connection.Query()
                .Include(x => x.Page)
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.Status == VersionStatus.Archived &&
                    x.CultureCode == currentCulture &&
                    x.Slug == slug)
                .OrderByDescending(x => x.DateModifiedUtc)
                .FirstOrDefaultAsync();
        }

        // If there isn't one...
        if (pageVersion == null)
        {
            // ...then try get the latest published version for the invariant culture
            using var connection = pageVersionService.OpenConnection();
            pageVersion = await connection.Query()
                .Include(x => x.Page)
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.Status == VersionStatus.Published &&
                    x.CultureCode == null &&
                    x.Slug == slug)
                .OrderByDescending(x => x.DateModifiedUtc)
                .FirstOrDefaultAsync();
        }

        // If there isn't one...
        if (pageVersion == null)
        {
            // ...then try get the last archived one for the invariant culture (TODO: What if last archived was a draft??)
            using var connection = pageVersionService.OpenConnection();
            pageVersion = await connection.Query()
                .Include(x => x.Page)
                .Where(x =>
                    x.TenantId == tenantId &&
                    x.Status == VersionStatus.Archived &&
                    x.CultureCode == null &&
                    x.Slug == slug)
                .OrderByDescending(x => x.DateModifiedUtc)
                .FirstOrDefaultAsync();
        }

        if (pageVersion != null && pageVersion.Page.IsEnabled)
        {
            // If there are access restrictions
            if (!await PageSecurityHelper.CheckUserHasAccessToPage(pageVersion.Page, User))
            {
                return Unauthorized();
            }

            // Else no restrictions (available for anyone to view)
            WorkContext.SetState("CurrentPageId", pageVersion.PageId);
            WorkContext.Breadcrumbs.Add(pageVersion.Title);

            var pageType = await pageTypeService.FindOneAsync(pageVersion.Page.PageTypeId);
            var mantlePageType = pageTypeService.GetMantlePageType(pageType.Name);
            mantlePageType.InstanceName = pageVersion.Title;
            mantlePageType.InstanceParentId = pageVersion.Page.ParentId;

            mantlePageType.LayoutPath = string.IsNullOrWhiteSpace(pageType.LayoutPath)
                ? MantleWebConstants.DefaultFrontendLayoutPath
                : pageType.LayoutPath;

            mantlePageType.InitializeInstance(pageVersion);

            var contentBlocks = contentBlockService.GetContentBlocks(pageVersion.PageId, WorkContext.CurrentCultureCode);
            mantlePageType.ReplaceContentTokens(x => InsertContentBlocks(x, contentBlocks.Where(y => IsVisible(y))));

            return View(mantlePageType.DisplayTemplatePath, mantlePageType);
        }

        return NotFound();
    }

    private bool IsVisible(IContentBlock contentBlock)
    {
        if (contentBlock == null || !contentBlock.Enabled)
        {
            return false;
        }

        return true;
    }

    private string InsertContentBlocks(string content, IEnumerable<IContentBlock> contentBlocks)
    {
        if (string.IsNullOrEmpty(content))
        {
            return null;
        }

        foreach (Match match in ContentZonePattern.Matches(content))
        {
            string zoneName = match.Groups["Zone"].Value;

            int tenantId = WorkContext.CurrentTenant.Id;
            var zone = zoneRepository.FindOne(x => x.TenantId == tenantId && x.Name == zoneName);

            if (zone == null)
            {
                zoneRepository.Insert(new Zone
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = zoneName
                });
                continue;
            }

            var contentBlocksByZone = contentBlocks.Where(x => x.ZoneId == zone.Id).ToList();
            var contentBlocksByZoneForAllPages = contentBlockService.GetContentBlocks(zone.Name, WorkContext.CurrentCultureCode);
            contentBlocksByZone.AddRange(contentBlocksByZoneForAllPages);

            string html = AsyncHelper.RunSync(() => razorViewRenderService.RenderToStringAsync(
                "Mantle.Web.ContentManagement.Views.Shared.Components.ContentBlocksByZone.Default.cshtml",
                contentBlocksByZone));

            content = content.Replace(match.Value, html);
        }
        return content;
    }
}