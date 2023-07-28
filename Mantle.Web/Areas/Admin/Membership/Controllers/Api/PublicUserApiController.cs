namespace Mantle.Web.Areas.Admin.Membership.Controllers.Api;

public class PublicUserApiController : ODataController
{
    private readonly IWorkContext workContext;

    protected IMembershipService Service { get; private set; }

    public PublicUserApiController(IMembershipService service, IWorkContext workContext)
    {
        this.Service = service;
        this.workContext = workContext;
    }

    public virtual async Task<IActionResult> Get(ODataQueryOptions<PublicUserInfo> options)
    {
        var query = (await Service.GetAllUsers(workContext.CurrentTenant.Id))
            .Select(x => new PublicUserInfo
            {
                Id = x.Id,
                UserName = x.UserName
            }).AsQueryable();

        var results = options.ApplyTo(query);
        var response = await Task.FromResult((results as IQueryable<PublicUserInfo>).ToHashSet());
        return Ok(response);
    }

    public virtual async Task<PublicUserInfo> Get([FromODataUri] string key)
    {
        var entity = await Service.GetUserById(key);
        return new PublicUserInfo
        {
            Id = entity.Id,
            UserName = entity.UserName
        };
    }
}

public class PublicUserInfo
{
    public string Id { get; set; }

    public string UserName { get; set; }
}