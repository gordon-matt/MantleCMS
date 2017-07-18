using Mantle.Identity;
using MantleCMS.Data.Domain;
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