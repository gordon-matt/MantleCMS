using System.IO;
using System.Threading.Tasks;
using Mantle.Helpers;
using Mantle.Security.Membership;
using Mantle.Tenants.Domain;
using Mantle.Tenants.Services;
using Mantle.Web.OData;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Areas.Admin.Tenants.Controllers.Api
{
    public class TenantApiController : GenericODataController<Tenant, int>
    {
        private readonly IMembershipService membershipService;
        private readonly IWebHelper webHelper;

        public TenantApiController(
            ITenantService service,
            IMembershipService membershipService,
            IWebHelper webHelper)
            : base(service)
        {
            this.membershipService = membershipService;
            this.webHelper = webHelper;
        }

        public override async Task<IActionResult> Post(Tenant entity)
        {
            var result = await base.Post(entity);
            int tenantId = entity.Id; // EF should have populated the ID in base.Post()
            await membershipService.EnsureAdminRoleForTenant(tenantId);

            //TOOD: Create tenant media folder:
            var mediaFolder = new DirectoryInfo(CommonHelper.MapPath("~/Media/Uploads/Tenant_" + tenantId));
            if (!mediaFolder.Exists)
            {
                mediaFolder.Create();
            }

            return result;
        }

        public override async Task<IActionResult> Delete(int key)
        {
            var result = await base.Delete(key);

            //TODO: Remove everything associated with the tenant.

            // TODO: Add some checkbox on admin page... only delete files if user checks that box.
            //var mediaFolder = new DirectoryInfo(webHelper.MapPath("~/Media/Uploads/Tenant_" + key));
            //if (mediaFolder.Exists)
            //{
            //    mediaFolder.Delete();
            //}

            return result;
        }

        protected override int GetId(Tenant entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(Tenant entity)
        {
        }

        protected override Permission ReadPermission
        {
            get { return StandardPermissions.FullAccess; }
        }

        protected override Permission WritePermission
        {
            get { return StandardPermissions.FullAccess; }
        }
    }
}