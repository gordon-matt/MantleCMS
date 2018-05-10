using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mantle.Collections;
using Mantle.Infrastructure;
using Mantle.Security;
using Mantle.Security.Membership;
using Mantle.Security.Membership.Permissions;
using Mantle.Web.Security.Membership;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mantle.Web.Areas.Admin.Membership.Controllers.Api
{
    public class UserApiController : ODataController
    {
        private readonly ILogger logger;
        private readonly IWorkContext workContext;

        protected IMembershipService Service { get; private set; }

        private readonly Lazy<MembershipSettings> membershipSettings;

        public UserApiController(
            IMembershipService service,
            Lazy<MembershipSettings> membershipSettings,
            ILoggerFactory loggerFactory,
            IWorkContext workContext)
        {
            this.Service = service;
            this.membershipSettings = membershipSettings;
            this.logger = loggerFactory.CreateLogger<UserApiController>();
            this.workContext = workContext;
        }

        public virtual async Task<IEnumerable<MantleUser>> Get(ODataQueryOptions<MantleUser> options)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipUsersRead))
            {
                return Enumerable.Empty<MantleUser>();
            }

            //var results = options.ApplyTo(Service.GetAllUsersAsQueryable(workContext.CurrentTenant.Id), AllowedQueryOptions.All);
            var query = (await Service.GetAllUsers(workContext.CurrentTenant.Id)).AsQueryable();
            var results = options.ApplyTo(query);
            return (results as IQueryable<MantleUser>).ToHashSet();
        }

        [EnableQuery]
        public virtual async Task<SingleResult<MantleUser>> Get([FromODataUri] string key)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipUsersRead))
            {
                return SingleResult.Create(Enumerable.Empty<MantleUser>().AsQueryable());
            }
            var entity = await Service.GetUserById(key);
            return SingleResult.Create(new[] { entity }.AsQueryable());
        }

        public virtual async Task<IActionResult> Put([FromODataUri] string key, [FromBody] MantleUser entity)
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

                if (!await CheckEntityExistsAsync(key))
                {
                    return NotFound();
                }
                else { throw; }
            }

            return Updated(entity);
        }

        public virtual async Task<IActionResult> Post([FromBody] MantleUser entity)
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
                membershipSettings.Value.GeneratedPasswordLength,
                membershipSettings.Value.GeneratedPasswordNumberOfNonAlphanumericChars);

            entity.TenantId = workContext.CurrentTenant.Id;
            await Service.InsertUser(entity, password);

            return Created(entity);
        }

        [AcceptVerbs("PATCH", "MERGE")]
        public virtual async Task<IActionResult> Patch([FromODataUri] string key, Delta<MantleUser> patch)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipUsersWrite))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MantleUser entity = await Service.GetUserById(key);
            if (entity == null)
            {
                return NotFound();
            }

            patch.Patch(entity);

            try
            {
                await Service.UpdateUser(entity);
            }
            catch (DbUpdateConcurrencyException x)
            {
                logger.LogError(new EventId(), x, x.Message);

                if (!await CheckEntityExistsAsync(key))
                {
                    return NotFound();
                }
                else { throw; }
            }

            return Updated(entity);
        }

        public virtual async Task<IActionResult> Delete([FromODataUri] string key)
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

        protected virtual async Task<bool> CheckEntityExistsAsync(string key)
        {
            var user = await Service.GetUserById(key);
            return user != null;
        }

        public virtual async Task<IEnumerable<MantleUser>> GetUsersInRole(
            [FromODataUri] string roleId,
            ODataQueryOptions<MantleUser> options)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipUsersRead))
            {
                return Enumerable.Empty<MantleUser>();
            }

            var query = (await Service.GetUsersByRoleId(roleId)).AsQueryable();
            var results = options.ApplyTo(query);
            return (results as IQueryable<MantleUser>).ToHashSet();
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssignUserToRoles([FromBody] ODataActionParameters parameters)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipUsersWrite))
            {
                return Unauthorized();
            }

            string userId = (string)parameters["userId"];
            var roleIds = (IEnumerable<string>)parameters["roles"];

            await Service.AssignUserToRoles(workContext.CurrentTenant.Id, userId, roleIds);

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ChangePassword([FromBody] ODataActionParameters parameters)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipUsersWrite))
            {
                return Unauthorized();
            }

            string userId = (string)parameters["userId"];
            string password = (string)parameters["password"];

            try
            {
                await Service.ChangePassword(userId, password);
                return Ok();
            }
            catch (Exception x)
            {
                return StatusCode(500, x.Message);
            }
        }

        protected static bool CheckPermission(Permission permission)
        {
            var authorizationService = EngineContext.Current.Resolve<IAuthorizationService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            return authorizationService.TryCheckAccess(permission, workContext.CurrentUser);
        }
    }
}