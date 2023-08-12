using Mantle.Web.ContentManagement.Areas.Admin.Pages.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages.Controllers.Api;

public class PageApiController : GenericTenantODataController<Page, Guid>
{
    private readonly IPageVersionService pageVersionService;
    private readonly IWorkContext workContext;

    public PageApiController(
        IPageService service,
        IPageVersionService pageVersionService,
        IWorkContext workContext)
        : base(service)
    {
        this.pageVersionService = pageVersionService;
        this.workContext = workContext;
    }

    protected override Guid GetId(Page entity)
    {
        return entity.Id;
    }

    protected override void SetNewId(Page entity)
    {
        entity.Id = Guid.NewGuid();
    }

    protected override Permission ReadPermission
    {
        get { return CmsPermissions.PagesRead; }
    }

    protected override Permission WritePermission
    {
        get { return CmsPermissions.PagesWrite; }
    }

    [HttpGet]
    public IActionResult GetTopLevelPages()
    {
        if (!CheckPermission(ReadPermission))
        {
            return Unauthorized();
        }

        int tenantId = GetTenantId();
        var topLevelPages = (Service as IPageService).GetTopLevelPages(tenantId);

        return Ok(topLevelPages);
    }
}