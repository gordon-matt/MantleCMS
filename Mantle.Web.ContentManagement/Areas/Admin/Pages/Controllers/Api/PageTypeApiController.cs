using Mantle.Web.ContentManagement.Areas.Admin.Pages.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages.Controllers.Api;

public class PageTypeApiController : GenericODataController<PageType, Guid>
{
    public PageTypeApiController(IPageTypeService service)
        : base(service)
    {
    }

    protected override Guid GetId(PageType entity)
    {
        return entity.Id;
    }

    protected override void SetNewId(PageType entity)
    {
        entity.Id = Guid.NewGuid();
    }

    protected override Permission ReadPermission
    {
        get { return CmsPermissions.PageTypesRead; }
    }

    protected override Permission WritePermission
    {
        get { return CmsPermissions.PageTypesWrite; }
    }
}