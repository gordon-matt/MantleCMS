using System.Threading.Tasks;
using Mantle.Security.Membership;
using Mantle.Tenants.Domain;
using Mantle.Tenants.Services;
using Mantle.Web.Mvc;
using Mantle.Web.Mvc.KendoUI;
using Mantle.Web.Security.Membership.Permissions;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Areas.Admin.Tenants.Controllers
{
    [Area(MantleWebConstants.Areas.Tenants)]
    [Route("api/tenants")]
    public class TenantApiController : MantleGenericDataController<Tenant, int>
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

        [HttpPost]
        [Route("get")]
        public override async Task<IActionResult> Get([FromBody]KendoGridMvcRequest request)
        {
            return await base.Get(request);
        }

        [HttpGet]
        [Route("{key}")]
        public override async Task<IActionResult> Get(int key)
        {
            return await base.Get(key);
        }

        [HttpPut]
        [Route("{key}")]
        public override async Task<IActionResult> Put(int key, [FromBody]Tenant entity)
        {
            return await base.Put(key, entity);
        }

        [HttpPost]
        [Route("")]
        public override async Task<IActionResult> Post([FromBody]Tenant entity)
        {
            var result = await base.Post(entity);
            int tenantId = entity.Id; // EF should have populated the ID in base.Post()
            await membershipService.EnsureAdminRoleForTenant(tenantId);

            //var mediaFolder = new DirectoryInfo(webHelper.MapPath("~/Media/Uploads/Tenant_" + tenantId));
            //if (!mediaFolder.Exists)
            //{
            //    mediaFolder.Create();
            //}

            return result;
        }

        [HttpDelete]
        [Route("{key}")]
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