using Mantle.Identity;
using Microsoft.AspNetCore.Identity;

namespace MantleCMS.Identity;

public class ApplicationRoleStore : MantleRoleStore<ApplicationRole, ApplicationDbContext>
{
    public ApplicationRoleStore(ApplicationDbContext context, IdentityErrorDescriber describer = null)
        : base(context, describer)
    {
    }
}