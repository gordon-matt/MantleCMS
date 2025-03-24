using Mantle.Web.ContentManagement.Areas.Admin.Menus.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Menus.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.Menus.Controllers.Api;

//[Authorize(Roles = MantleConstants.Roles.Administrators)]
public class MenuItemApiController : GenericODataController<MenuItem, Guid>
{
    public MenuItemApiController(IMenuItemService service)
        : base(service)
    {
    }

    protected override Guid GetId(MenuItem entity) => entity.Id;

    protected override void SetNewId(MenuItem entity) => entity.Id = Guid.NewGuid();

    protected override Permission ReadPermission => CmsPermissions.MenusRead;

    protected override Permission WritePermission => CmsPermissions.MenusWrite;
}