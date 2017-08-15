using System.Collections.Generic;
using Mantle.Identity.Domain;

namespace MantleCMS.Data.Domain
{
    public class ApplicationRole : MantleIdentityRole
    {
        public virtual ICollection<RolePermission> RolesPermissions { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}