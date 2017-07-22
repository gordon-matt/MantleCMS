using Mantle.Configuration.Domain;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Identity.Domain;
using Mantle.Infrastructure;
using Mantle.Localization.Domain;
using Mantle.Logging.Domain;
using Mantle.Tasks.Domain;
using Mantle.Tenants.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LanguageEntity = Mantle.Localization.Domain.Language;

namespace Mantle.Identity
{
    public abstract class MantleIdentityDbContext<TUser, TRole>
        : IdentityDbContext<TUser, TRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
        where TUser : MantleIdentityUser
        where TRole : MantleIdentityRole
    {
        #region Constructors

        public MantleIdentityDbContext(DbContextOptions options)
            : base(options)
        {
        }

        #endregion Constructors

        public DbSet<LanguageEntity> Languages { get; set; }

        public DbSet<LocalizableProperty> LocalizableProperties { get; set; }

        public DbSet<LocalizableString> LocalizableStrings { get; set; }

        public DbSet<LogEntry> Log { get; set; }

        public DbSet<ScheduledTask> ScheduledTasks { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //var usersTable = modelBuilder.Entity<TUser>().ToTable("AspNetUsers");
            //usersTable.HasMany(x => x.Roles).WithRequired().HasForeignKey(x => x.UserId);
            //usersTable.HasMany(x => x.Claims).WithRequired().HasForeignKey(x => x.UserId);
            //usersTable.HasMany(x => x.Logins).WithRequired().HasForeignKey(x => x.UserId);

            //usersTable.Property(x => x.TenantId)
            //    .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true, Order = 1 }));

            //usersTable.Property(x => x.UserName)
            //    .IsRequired()
            //    .HasMaxLength(256)
            //    .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true, Order = 2 }));

            //usersTable.Property(x => x.Email).HasMaxLength(256);

            //modelBuilder.Entity<IdentityUserRole>().HasKey(x => new
            //{
            //    UserId = x.UserId,
            //    RoleId = x.RoleId
            //}).ToTable("AspNetUserRoles");

            //modelBuilder.Entity<IdentityUserLogin>().HasKey(x => new
            //{
            //    LoginProvider = x.LoginProvider,
            //    ProviderKey = x.ProviderKey,
            //    UserId = x.UserId
            //}).ToTable("AspNetUserLogins");

            //modelBuilder.Entity<IdentityUserClaim>().ToTable("AspNetUserClaims");

            //var rolesTable = modelBuilder.Entity<TRole>().ToTable("AspNetRoles");

            //rolesTable.Property(x => x.TenantId)
            //    .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("RoleNameIndex") { IsUnique = true, Order = 1 }));

            //rolesTable.Property(x => x.Name)
            //    .IsRequired()
            //    .HasMaxLength(256)
            //    .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("RoleNameIndex") { IsUnique = true, Order = 2 }));

            //rolesTable.HasMany(x => x.Users).WithRequired().HasForeignKey(x => x.RoleId);

            var configurations = EngineContext.Current.ResolveAll<IEntityTypeConfiguration>();

            foreach (var configuration in configurations)
            {
                configuration.Configure(modelBuilder);
            }
        }
    }
}