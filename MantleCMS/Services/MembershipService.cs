using Microsoft.AspNetCore.Identity;

namespace MantleCMS.Services;

public class MembershipService : IdentityMembershipService
{
    public MembershipService(
        IDbContextFactory contextFactory,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IRepository<UserProfileEntry> userProfileRepository)
        : base(contextFactory, userManager, roleManager, userProfileRepository)
    {
    }
}