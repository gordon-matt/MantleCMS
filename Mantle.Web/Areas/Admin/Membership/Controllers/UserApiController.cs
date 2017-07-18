using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mantle.Infrastructure;
using Mantle.Security;
using Mantle.Security.Membership;
using Mantle.Threading;
using Mantle.Web.Mvc.KendoUI;
using Mantle.Web.Security.Membership;
using Mantle.Web.Security.Membership.Permissions;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Mantle.Web.Areas.Admin.Membership.Controllers
{
    [Route("api/membership/users")]
    public class UserApiController : Controller
    {
        private readonly ILogger logger;
        private readonly IWorkContext workContext;

        protected IMembershipService Service { get; private set; }

        private readonly MembershipSettings membershipSettings;

        public UserApiController(
            IMembershipService service,
            MembershipSettings membershipSettings,
            ILoggerFactory loggerFactory,
            IWorkContext workContext)
        {
            this.Service = service;
            this.membershipSettings = membershipSettings;
            this.logger = loggerFactory.CreateLogger<UserApiController>();
            this.workContext = workContext;
        }

        [HttpPost]
        [Route("get")]
        public virtual async Task<IActionResult> Get([FromBody]KendoGridMvcRequest request)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipUsersRead))
            {
                return Unauthorized();
            }

            //var query = Service.GetAllUsersAsQueryable(workContext.CurrentTenant.Id);
            var query = (await Service.GetAllUsers(workContext.CurrentTenant.Id)).AsQueryable();

            var grid = new CustomKendoGridEx<MantleUser>(request, query);
            return Json(grid);
        }

        [HttpGet]
        [Route("{key}")]
        public virtual async Task<IActionResult> Get(string key)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipUsersRead))
            {
                return Unauthorized();
            }
            var entity = await Service.GetUserById(key);
            return Json(JObject.FromObject(entity));
        }

        [HttpPut]
        [Route("{key}")]
        public virtual async Task<IActionResult> Put(string key, [FromBody]MantleUser entity)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipUsersWrite))
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
                await Service.UpdateUser(entity);
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
        public virtual async Task<IActionResult> Post([FromBody]MantleUser entity)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipUsersWrite))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string password = Password.Generate(
                membershipSettings.GeneratedPasswordLength,
                membershipSettings.GeneratedPasswordNumberOfNonAlphanumericChars);

            entity.TenantId = workContext.CurrentTenant.Id;
            await Service.InsertUser(entity, password);

            return Ok(entity);
        }

        [HttpDelete]
        [Route("{key}")]
        public virtual async Task<IActionResult> Delete(string key)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipUsersWrite))
            {
                return Unauthorized();
            }

            MantleUser entity = await Service.GetUserById(key);
            if (entity == null)
            {
                return NotFound();
            }

            await Service.DeleteUser(key);

            return NoContent();
        }

        protected virtual bool EntityExists(string key)
        {
            return AsyncHelper.RunSync(() => Service.GetUserById(key)) != null;
        }

        [HttpPost]
        [Route("Default.GetUsersInRole")]
        public virtual async Task<IActionResult> GetUsersInRole([FromBody]dynamic data)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipUsersRead))
            {
                return Unauthorized();
            }

            // TODO: Test
            string roleId = data.roleId;
            KendoGridMvcRequest request = data;
            var query = (await Service.GetUsersByRoleId(roleId)).AsQueryable();

            var grid = new CustomKendoGridEx<MantleUser>(request, query);
            return Json(grid);
        }

        [HttpPost]
        [Route("Default.AssignUserToRoles")]
        public virtual async Task<IActionResult> AssignUserToRoles([FromBody]dynamic data)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipUsersWrite))
            {
                return Unauthorized();
            }

            string userId = data.userId;
            string[] roles = data.roles.ToObject<string[]>();
            await Service.AssignUserToRoles(workContext.CurrentTenant.Id, userId, roles);

            return Ok();
        }

        [HttpPost]
        [Route("Default.ChangePassword")]
        public virtual async Task<IActionResult> ChangePassword([FromBody]dynamic data)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipUsersWrite))
            {
                return Unauthorized();
            }

            string userId = data.userId;
            string password = data.password;
            await Service.ChangePassword(userId, password);

            return Ok();
        }

        protected static bool CheckPermission(Permission permission)
        {
            var authorizationService = EngineContext.Current.Resolve<IAuthorizationService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            return authorizationService.TryCheckAccess(permission, workContext.CurrentUser);
        }
    }
}