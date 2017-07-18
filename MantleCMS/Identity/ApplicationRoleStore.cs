using Mantle.Identity;
using MantleCMS.Data;
using MantleCMS.Data.Domain;
using Microsoft.AspNetCore.Identity;

namespace MantleCMS.Identity
{
    public class ApplicationRoleStore : MantleRoleStore<ApplicationRole, ApplicationDbContext>
    {
        public ApplicationRoleStore(ApplicationDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
        }
    }
}