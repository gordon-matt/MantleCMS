using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mantle.Collections;
using Mantle.Infrastructure;
using Mantle.Security.Membership;
using Mantle.Threading;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mantle.Web.Areas.Admin.Membership.Controllers.Api
{
    public class RoleApiController : ODataController
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
        
        public virtual async Task<IEnumerable<MantleRole>> Get(ODataQueryOptions<MantleRole> options)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipRolesRead))
            {
                return Enumerable.Empty<MantleRole>().AsQueryable();
            }

            //var settings = new ODataValidationSettings()
            //{
            //    AllowedQueryOptions = AllowedQueryOptions.All
            //};
            //options.Validate(settings);

            var results = options.ApplyTo((await Service.GetAllRoles(workContext.CurrentTenant.Id)).AsQueryable());
            return (results as IQueryable<MantleRole>).ToHashSet();
        }
        
        public virtual async Task<SingleResult<MantleRole>> Get([FromODataUri] string key)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipRolesRead))
            {
                return SingleResult.Create(Enumerable.Empty<MantleRole>().AsQueryable());
            }
            var entity = await Service.GetRoleById(key);
            return SingleResult.Create(new[] { entity }.AsQueryable());
        }

        public virtual async Task<IActionResult> Put([FromODataUri] string key, MantleRole entity)
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

            return Updated(entity);
        }

        public virtual async Task<IActionResult> Post(MantleRole entity)
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

            return Created(entity);
        }

        [AcceptVerbs("PATCH", "MERGE")]
        public virtual async Task<IActionResult> Patch([FromODataUri] string key, Delta<MantleRole> patch)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipRolesWrite))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MantleRole entity = await Service.GetRoleById(key);
            if (entity == null)
            {
                return NotFound();
            }

            patch.Patch(entity);

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

            return Updated(entity);
        }

        public virtual async Task<IActionResult> Delete([FromODataUri] string key)
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
        public virtual async Task<IEnumerable<EdmRole>> GetRolesForUser([FromBody] ODataActionParameters parameters)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipRolesRead))
            {
                return Enumerable.Empty<EdmRole>().AsQueryable();
            }
            string userId = (string)parameters["userId"];
            return (await Service.GetRolesForUser(userId)).Select(x => new EdmRole
            {
                Id = x.Id,
                Name = x.Name
            });
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssignPermissionsToRole([FromBody] ODataActionParameters parameters)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipRolesWrite))
            {
                return Unauthorized();
            }

            string roleId = (string)parameters["roleId"];
            var permissionIds = (IEnumerable<string>)parameters["permissions"];

            await Service.AssignPermissionsToRole(roleId, permissionIds);

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