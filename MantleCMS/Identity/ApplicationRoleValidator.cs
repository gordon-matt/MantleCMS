using Mantle.Identity;
using Microsoft.AspNetCore.Identity;

namespace MantleCMS.Identity
{
    public class ApplicationRoleValidator : MantleRoleValidator<ApplicationRole>
    {
        public ApplicationRoleValidator(IdentityErrorDescriber errors = null)
            : base(errors)
        {
        }
    }
}