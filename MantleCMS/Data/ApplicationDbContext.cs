using Mantle.Identity;
using MantleCMS.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace MantleCMS.Data
{
    public class ApplicationDbContext : MantleIdentityDbContext<ApplicationUser, ApplicationRole>
    {
        public DbSet<Permission> Permissions { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<UserProfileEntry> UserProfiles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}