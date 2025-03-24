using Microsoft.EntityFrameworkCore;

namespace Mantle.Web.Areas.Admin.Membership.Controllers.Api;

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

    public virtual async Task<IActionResult> Get(ODataQueryOptions<MantleRole> options)
    {
        if (!CheckPermission(MantleWebPermissions.MembershipRolesRead))
        {
            return Unauthorized();
        }

        //var settings = new ODataValidationSettings()
        //{
        //    AllowedQueryOptions = AllowedQueryOptions.All
        //};
        //options.Validate(settings);

        var results = options.ApplyTo((await Service.GetAllRoles(workContext.CurrentTenant.Id)).AsQueryable());
        var response = await Task.FromResult((results as IQueryable<MantleRole>).ToHashSet());
        return Ok(response);
    }

    [EnableQuery]
    public virtual async Task<IActionResult> Get([FromODataUri] string key)
    {
        if (!CheckPermission(MantleWebPermissions.MembershipRolesRead))
        {
            return Unauthorized();
        }

        var entity = await Service.GetRoleById(key);

        return entity == null ? NotFound() : Ok(entity);
    }

    public virtual async Task<IActionResult> Put([FromODataUri] string key, [FromBody] MantleRole entity)
    {
        if (entity == null)
        {
            return BadRequest();
        }

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

    public virtual async Task<IActionResult> Post([FromBody] MantleRole entity)
    {
        if (entity == null)
        {
            return BadRequest();
        }

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

        var entity = await Service.GetRoleById(key);
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

        var entity = await Service.GetRoleById(key);
        if (entity == null)
        {
            return NotFound();
        }

        await Service.DeleteRole(key);

        return NoContent();
    }

    [HttpGet]
    public virtual async Task<IActionResult> GetRolesForUser([FromODataUri] string userId)
    {
        if (!CheckPermission(MantleWebPermissions.MembershipRolesRead))
        {
            return Unauthorized();
        }

        var results = (await Service.GetRolesForUser(userId)).Select(x => new EdmRole
        {
            Id = x.Id,
            Name = x.Name
        });

        var response = await Task.FromResult(results);
        return Ok(response);
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

    protected virtual bool EntityExists(string key) => AsyncHelper.RunSync(() => Service.GetUserById(key)) != null;

    protected static bool CheckPermission(Permission permission)
    {
        var authorizationService = EngineContext.Current.Resolve<IMantleAuthorizationService>();
        var workContext = EngineContext.Current.Resolve<IWorkContext>();
        return authorizationService.TryCheckAccess(permission, workContext.CurrentUser);
    }
}

public struct EdmRole
{
    public string Id { get; set; }

    public string Name { get; set; }
}