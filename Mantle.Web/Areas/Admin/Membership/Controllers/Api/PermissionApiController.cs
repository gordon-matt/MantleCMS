using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mantle.Data.Entity;
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
    public class PermissionApiController : ODataController
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

        public virtual async Task<IEnumerable<MantlePermission>> Get(ODataQueryOptions<MantlePermission> options)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipPermissionsRead))
            {
                return Enumerable.Empty<MantlePermission>().AsQueryable();
            }

            var results = options.ApplyTo(
                (await Service.GetAllPermissions(workContext.CurrentTenant.Id)).AsQueryable());

            return await (results as IQueryable<MantlePermission>).ToHashSetAsync();
        }

        [EnableQuery]
        public virtual async Task<SingleResult<MantlePermission>> Get([FromODataUri] string key)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipPermissionsRead))
            {
                return SingleResult.Create(Enumerable.Empty<MantlePermission>().AsQueryable());
            }
            var entity = await Service.GetPermissionById(key);
            return SingleResult.Create(new[] { entity }.AsQueryable());
        }

        public virtual async Task<IActionResult> Put([FromODataUri] string key, MantlePermission entity)
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
                logger.LogError(new EventId(), x.Message, x);

                if (!EntityExists(key))
                {
                    return NotFound();
                }
                else { throw; }
            }

            return Updated(entity);
        }

        public virtual async Task<IActionResult> Post(MantlePermission entity)
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

            return Created(entity);
        }

        [AcceptVerbs("PATCH", "MERGE")]
        public virtual async Task<IActionResult> Patch([FromODataUri] string key, Delta<MantlePermission> patch)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipPermissionsWrite))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MantlePermission entity = await Service.GetPermissionById(key);
            if (entity == null)
            {
                return NotFound();
            }

            patch.Patch(entity);

            try
            {
                await Service.UpdatePermission(entity);
            }
            catch (DbUpdateConcurrencyException x)
            {
                logger.LogError(new EventId(), x.Message, x);

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

        [HttpGet]
        public virtual async Task<IEnumerable<EdmMantlePermission>> GetPermissionsForRole([FromODataUri] string roleId)
        {
            if (!CheckPermission(MantleWebPermissions.MembershipPermissionsRead))
            {
                return Enumerable.Empty<EdmMantlePermission>().AsQueryable();
            }

            var role = await Service.GetRoleById(roleId);
            return (await Service.GetPermissionsForRole(workContext.CurrentTenant.Id, role.Name)).Select(x => new EdmMantlePermission
            {
                Id = x.Id,
                Name = x.Name
            });
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

    public struct EdmMantlePermission
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}