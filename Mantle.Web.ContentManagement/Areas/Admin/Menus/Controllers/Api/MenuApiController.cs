using System;
using Mantle.Web.ContentManagement.Areas.Admin.Menus.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Menus.Services;
using Mantle.Web.OData;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.Areas.Admin.Menus.Controllers.Api
{
    //[Authorize(Roles = MantleConstants.Roles.Administrators)]
    [Route("api/menus")]
    public class MenuApiController : GenericTenantODataController<Menu, Guid>
    {
        public MenuApiController(IMenuService service)
            : base(service)
        {
        }

        protected override Guid GetId(Menu entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(Menu entity)
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
}