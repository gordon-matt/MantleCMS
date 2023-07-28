namespace Mantle.Web.Security.Membership;

public class CurrentUserStateProvider : IWorkContextStateProvider
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IMembershipService membershipService;

    public CurrentUserStateProvider(
        IHttpContextAccessor httpContextAccessor,
        IMembershipService membershipService)
    {
        this.membershipService = membershipService;
        this.httpContextAccessor = httpContextAccessor;
    }

    public Func<IWorkContext, T> Get<T>(string name)
    {
        if (name == MantleWebConstants.StateProviders.CurrentUser)
        {
            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return null;
            }

            if (httpContext.User.Identity.IsAuthenticated)
            {
                return ctx =>
                {
                    httpContext = httpContextAccessor.HttpContext;
                    var user = AsyncHelper.RunSync(() => membershipService.GetUserByName(ctx.CurrentTenant.Id, httpContext.User.Identity.Name));

                    if (user == null)
                    {
                        user = AsyncHelper.RunSync(() => membershipService.GetUserByName(null, httpContext.User.Identity.Name));
                    }

                    if (user == null)
                    {
                        return default;
                    }
                    return (T)(object)user;
                };
            }
        }
        return null;
    }
}