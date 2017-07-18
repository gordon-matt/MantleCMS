using System.Linq;
using System.Threading.Tasks;
using Mantle.Infrastructure;
using Mantle.Security.Membership;
using Mantle.Threading;
using Mantle.Web.Mvc.KendoUI;
using Mantle.Web.Security.Membership.Permissions;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Mantle.Web.Areas.Admin.Membership.Controllers
{
    [Route("api/membership/permissions")]
    public class PermissionApiController : Controller
    {
        private readonly ILogger logger;
        private readonly IWorkContext workContext;

        protected IMembershipService Service { get; private set; }

        public PermissionApiController(
            IMembershipService service,
            ILoggerFactory loggerFactory,
            IWorkContext workContext)
        {
            this.Service = service;
            this.logger = loggerFactory.CreateLogger<PermissionApiController>();
            this.workContext = workContext;
        }

        [HttpPost]
        [Route("get")]
        public virtual async Task<IActionResult> Get([FromBody]KendoGridMvcRequest request)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipPermissionsRead))
            {
                return Unauthorized();
            }

            var query = (await Service.GetAllPermissions(workContext.CurrentTenant.Id)).AsQueryable();

            var grid = new CustomKendoGridEx<MantlePermission>(request, query);
            return Json(grid);
        }

        [HttpGet]
        [Route("{key}")]
        public virtual async Task<IActionResult> Get(string key)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipPermissionsRead))
            {
                return Unauthorized();
            }
            var entity = await Service.GetPermissionById(key);
            return Json(JObject.FromObject(entity));
        }

        [HttpPut]
        [Route("{key}")]
        public virtual async Task<IActionResult> Put(string key, [FromBody]MantlePermission entity)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipPermissionsWrite))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!key.Equals(entity.Id))
            {
                return BadRequest();
            }

            try
            {
                await Service.UpdatePermission(entity);
            }
            catch (DbUpdateConcurrencyException x)
            {
                logger.LogError(new EventId(), x, x.Message);

                if (!EntityExists(key))
                {
                    return NotFound();
                }
                else { throw; }
            }

            return Ok(entity);
        }

        [HttpPost]
        [Route("")]
        public virtual async Task<IActionResult> Post([FromBody]MantlePermission entity)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipPermissionsWrite))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            entity.TenantId = workContext.CurrentTenant.Id;
            await Service.InsertPermission(entity);

            return Ok(entity);
        }

        [HttpDelete]
        [Route("{key}")]
        public virtual async Task<IActionResult> Delete(string key)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipPermissionsWrite))
            {
                return Unauthorized();
            }

            MantlePermission entity = await Service.GetPermissionById(key);
            if (entity == null)
            {
                return NotFound();
            }

            await Service.DeletePermission(key);

            return NoContent();
        }

        [HttpPost]
        [Route("Default.GetPermissionsForRole")]
        public virtual async Task<IActionResult> GetPermissionsForRole([FromBody]dynamic data)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipPermissionsRead))
            {
                return Unauthorized();
            }

            string roleId = data.roleId;
            var role = await Service.GetRoleById(roleId);
            var result = (await Service.GetPermissionsForRole(workContext.CurrentTenant.Id, role.Name)).Select(x => new EdmKorePermission
            {
                Id = x.Id,
                Name = x.Name
            });
            return Json(new JArray(result.Select(x => JObject.FromObject(x))));
        }

        protected virtual bool EntityExists(string key)
        {
            return AsyncHelper.RunSync(() => Service.GetUserById(key)) != null;
        }

        protected static bool CheckPermission(Permission permission)
        {
            var authorizationService = EngineContext.Current.Resolve<IAuthorizationService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            return authorizationService.TryCheckAccess(permission, workContext.CurrentUser);
        }
    }

    public struct EdmKorePermission
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}