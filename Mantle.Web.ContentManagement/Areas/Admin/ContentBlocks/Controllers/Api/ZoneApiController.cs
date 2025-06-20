﻿using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Controllers.Api;

//[Authorize(Roles = MantleConstants.Roles.Administrators)]
public class ZoneApiController : GenericTenantODataController<Zone, Guid>
{
    public ZoneApiController(IZoneService service)
        : base(service)
    {
    }

    protected override Guid GetId(Zone entity) => entity.Id;

    protected override void SetNewId(Zone entity) => entity.Id = Guid.NewGuid();

    protected override Permission ReadPermission => CmsPermissions.ContentZonesRead;

    protected override Permission WritePermission => CmsPermissions.ContentZonesWrite;
}