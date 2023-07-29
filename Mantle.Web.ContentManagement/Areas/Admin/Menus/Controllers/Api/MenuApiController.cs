using Mantle.Web.ContentManagement.Areas.Admin.Menus.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Menus.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.Menus.Controllers.Api;

//[Authorize(Roles = MantleConstants.Roles.Administrators)]
public class MenuApiController : GenericTenantODataController<Menu, Guid>
{
    public MenuApiController(IMenuService service)
        : base(service)
    {
    }

    protected override Guid GetId(Menu entity) => entity.Id;

    protected override void SetNewId(Menu entity) => entity.Id = Guid.NewGuid();

    protected override Permission ReadPermission => CmsPermissions.MenusRead;

    protected override Permission WritePermission => CmsPermissions.MenusWrite;
}