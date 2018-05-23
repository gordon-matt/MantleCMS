using System;
using Mantle.Security.Membership.Permissions;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;
using Mantle.Web.OData;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Controllers.Api
{
    //[Authorize(Roles = MantleConstants.Roles.Administrators)]
    public class ZoneApiController : GenericTenantODataController<Zone, Guid>
    {
        public ZoneApiController(IZoneService service)
            : base(service)
        {
        }

        protected override Guid GetId(Zone entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(Zone entity)
        {
            entity.Id = Guid.NewGuid();
        }

        protected override Permission ReadPermission
        {
            get { return CmsPermissions.ContentZonesRead; }
        }

        protected override Permission WritePermission
        {
            get { return CmsPermissions.ContentZonesWrite; }
        }
    }
}