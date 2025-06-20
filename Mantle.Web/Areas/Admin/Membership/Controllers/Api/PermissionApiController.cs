﻿using Microsoft.EntityFrameworkCore;

namespace Mantle.Web.Areas.Admin.Membership.Controllers.Api;

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
        Service = service;
        this.logger = loggerFactory.CreateLogger<PermissionApiController>();
        this.workContext = workContext;
    }

    public virtual async Task<IActionResult> Get(ODataQueryOptions<MantlePermission> options)
    {
        if (!CheckPermission(MantleWebPermissions.MembershipPermissionsRead))
        {
            return Unauthorized();
        }

        var results = options.ApplyTo(
            (await Service.GetAllPermissions(workContext.CurrentTenant.Id)).AsQueryable());

        var response = await Task.FromResult((results as IQueryable<MantlePermission>).ToHashSet());
        return Ok(response);
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

    public virtual async Task<IActionResult> Put([FromODataUri] string key, [FromBody] MantlePermission entity)
    {
        if (entity == null)
        {
            return BadRequest();
        }

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

    public virtual async Task<IActionResult> Post([FromBody] MantlePermission entity)
    {
        if (entity == null)
        {
            return BadRequest();
        }

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

        var entity = await Service.GetPermissionById(key);
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

        var entity = await Service.GetPermissionById(key);
        if (entity == null)
        {
            return NotFound();
        }

        await Service.DeletePermission(key);

        return NoContent();
    }

    [HttpGet]
    public virtual async Task<IActionResult> GetPermissionsForRole([FromODataUri] string roleId)
    {
        if (!CheckPermission(MantleWebPermissions.MembershipPermissionsRead))
        {
            return Unauthorized();
        }

        var role = await Service.GetRoleById(roleId);
        var results = (await Service.GetPermissionsForRole(workContext.CurrentTenant.Id, role.Name)).Select(x => new EdmMantlePermission
        {
            Id = x.Id,
            Name = x.Name
        });

        var response = await Task.FromResult(results);
        return Ok(response);
    }

    protected virtual bool EntityExists(string key) => AsyncHelper.RunSync(() => Service.GetUserById(key)) != null;

    protected static bool CheckPermission(Permission permission)
    {
        var authorizationService = DependoResolver.Instance.Resolve<IMantleAuthorizationService>();
        var workContext = DependoResolver.Instance.Resolve<IWorkContext>();
        return authorizationService.TryCheckAccess(permission, workContext.CurrentUser);
    }
}

public struct EdmMantlePermission
{
    public string Id { get; set; }

    public string Name { get; set; }
}