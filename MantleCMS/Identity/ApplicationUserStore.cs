using Mantle.Identity;
using Mantle.Identity.Domain;
using Microsoft.AspNetCore.Identity;

namespace MantleCMS.Identity;

public class ApplicationUserStore : ApplicationUserStore<ApplicationUser>
{
    public ApplicationUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null)
        : base(context, describer)
    {
    }
}

public abstract class ApplicationUserStore<TUser> : MantleUserStore<TUser, ApplicationRole, ApplicationDbContext>
    where TUser : MantleIdentityUser, new()
{
    public ApplicationUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null)
        : base(context, describer)
    {
    }
}