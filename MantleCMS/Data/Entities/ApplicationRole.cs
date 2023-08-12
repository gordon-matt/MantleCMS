using Mantle.Identity.Entities;

namespace MantleCMS.Data.Entities;

public class ApplicationRole : MantleIdentityRole
{
    public virtual ICollection<RolePermission> RolesPermissions { get; set; }

    public virtual ICollection<ApplicationUser> Users { get; set; }
}