using System.Linq;
using System.Threading.Tasks;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Mantle.Infrastructure;
using Mantle.Security.Membership;
using Mantle.Threading;
using Mantle.Web.Mvc.KendoUI;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Mantle.Web.Areas.Admin.Membership.Controllers
{
    [Route("api/membership/roles")]
    public class RoleApiController : Controller
    {
        private readonly ILogger logger;
        private readonly IWorkContext workContext;

        protected IMembershipService Service { get; private set; }

        public RoleApiController(
            IMembershipService service,
            ILoggerFactory loggerFactory,
            IWorkContext workContext)
        {
            this.Service = service;
            this.logger = loggerFactory.CreateLogger<RoleApiController>();
            this.workContext = workContext;
        }

        [HttpPost]
        [Route("get")]
        public virtual async Task<IActionResult> Get([FromBody]KendoGridMvcRequest request)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipRolesRead))
            {
                return Unauthorized();
            }

            var query = (await Service.GetAllRoles(workContext.CurrentTenant.Id)).AsQueryable();

            var grid = new CustomKendoGridEx<MantleRole>(request, query);
            return Json(grid);
        }

        [HttpGet]
        [Route("{key}")]
        public virtual async Task<IActionResult> Get(string key)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipRolesRead))
            {
                return Unauthorized();
            }
            var entity = await Service.GetRoleById(key);
            return Json(JObject.FromObject(entity));
        }

        [HttpPut]
        [Route("{key}")]
        public virtual async Task<IActionResult> Put(string key, [FromBody]MantleRole entity)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipRolesWrite))
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
                await Service.UpdateRole(entity);
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
        public virtual async Task<IActionResult> Post([FromBody]MantleRole entity)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipRolesWrite))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            entity.TenantId = workContext.CurrentTenant.Id;
            await Service.InsertRole(entity);

            return Ok(entity);
        }

        [HttpDelete]
        [Route("{key}")]
        public virtual async Task<IActionResult> Delete(string key)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipRolesWrite))
            {
                return Unauthorized();
            }

            MantleRole entity = await Service.GetRoleById(key);
            if (entity == null)
            {
                return NotFound();
            }

            await Service.DeleteRole(key);

            return NoContent();
        }

        [HttpPost]
        [Route("Default.GetRolesForUser")]
        public virtual async Task<IActionResult> GetRolesForUser([FromBody]dynamic data)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipRolesRead))
            {
                return Unauthorized();
            }
            string userId = data.userId;
            var result = (await Service.GetRolesForUser(userId)).Select(x => new EdmRole
            {
                Id = x.Id,
                Name = x.Name
            });
            return Json(JArray.FromObject(result));
        }

        [HttpPost]
        [Route("Default.AssignPermissionsToRole")]
        public virtual async Task<IActionResult> AssignPermissionsToRole([FromBody]dynamic data)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipRolesWrite))
            {
                return Unauthorized();
            }

            string roleId = data.roleId;
            string[] permissions = data.permissions.ToObject<string[]>();
            await Service.AssignPermissionsToRole(roleId, permissions);

            return Ok();
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

    public struct EdmRole
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}