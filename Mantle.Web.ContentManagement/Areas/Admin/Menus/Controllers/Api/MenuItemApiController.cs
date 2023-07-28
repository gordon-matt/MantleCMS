using Mantle.Web.ContentManagement.Areas.Admin.Menus.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Menus.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.Menus.Controllers.Api;

//[Authorize(Roles = MantleConstants.Roles.Administrators)]
public class MenuItemApiController : GenericODataController<MenuItem, Guid>
{
    public MenuItemApiController(IMenuItemService service)
        : base(service)
    {
    }

    protected override Guid GetId(MenuItem entity)
    {
        return entity.Id;
    }

    protected override void SetNewId(MenuItem entity)
    {
        entity.Id = Guid.NewGuid();
    }

    protected override Permission ReadPermission
    {
        get { return CmsPermissions.MenusRead; }
    }

    protected override Permission WritePermission
    {
        get { return CmsPermissions.MenusWrite; }
    }
}