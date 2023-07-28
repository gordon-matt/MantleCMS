using Mantle.Security;
using Mantle.Web.Security.Membership;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Web.Areas.Admin.Membership.Controllers.Api;

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

    public virtual async Task<IActionResult> Get(ODataQueryOptions<MantleUser> options)
    {
        if (!CheckPermission(MantleWebPermissions.MembershipUsersRead))
        {
            return Unauthorized();
        }

        var query = (await Service.GetAllUsers(workContext.CurrentTenant.Id)).AsQueryable();
        var results = options.ApplyTo(query);

        var response = await Task.FromResult((results as IQueryable<MantleUser>).ToHashSet());
        return Ok(response);
    }

    [EnableQuery]
    public virtual async Task<IActionResult> Get([FromODataUri] string key)
    {
        if (!CheckPermission(MantleWebPermissions.MembershipUsersRead))
        {
            return Unauthorized();
        }

        var entity = await Service.GetUserById(key);

        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    public virtual async Task<IActionResult> Put([FromODataUri] string key, [FromBody] MantleUser entity)
    {
        if (entity == null)
        {
            return BadRequest();
        }

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
        if (entity == null)
        {
            return BadRequest();
        }

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

        var entity = await Service.GetUserById(key);
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

        var entity = await Service.GetUserById(key);
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

    public virtual async Task<IActionResult> GetUsersInRole(
        [FromODataUri] string roleId,
        ODataQueryOptions<MantleUser> options)
    {
        if (!CheckPermission(MantleWebPermissions.MembershipUsersRead))
        {
            return Unauthorized();
        }

        var query = (await Service.GetUsersByRoleId(roleId)).AsQueryable();
        var results = options.ApplyTo(query);

        var response = await Task.FromResult((results as IQueryable<MantleUser>).ToHashSet());
        return Ok(response);
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
        var authorizationService = EngineContext.Current.Resolve<IMantleAuthorizationService>();
        var workContext = EngineContext.Current.Resolve<IWorkContext>();
        return authorizationService.TryCheckAccess(permission, workContext.CurrentUser);
    }
}