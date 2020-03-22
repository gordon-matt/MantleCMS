using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mantle.Data.Entity;
using Mantle.Helpers;
using Mantle.Identity.Domain;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Localization.Domain;
using Mantle.Logging.Domain;
using Mantle.Tasks.Domain;
using Mantle.Tenants.Domain;
using Mantle.Web.Configuration.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LanguageEntity = Mantle.Localization.Domain.Language;

namespace Mantle.Identity
{
    public abstract class MantleIdentityDbContext<TUser, TRole>
        : IdentityDbContext<TUser, TRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>,
        ISupportSeed
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

            var configurations = EngineContext.Current.ResolveAll<IMantleEntityTypeConfiguration>();

            foreach (dynamic configuration in configurations)
            {
                modelBuilder.ApplyConfiguration(configuration);
            }
        }

        #region ISupportSeed Members

        public virtual void Seed()
        {
            var tenant = new Tenant
            {
                Name = "Default",
                Url = "my-domain.com",
                Hosts = "my-domain.com"
            };

            // Create default tenant
            Tenants.Add(tenant);
            SaveChanges();

            var mediaFolder = new DirectoryInfo(CommonHelper.MapPath("~/Media/Uploads/Tenant_" + tenant.Id));
            if (!mediaFolder.Exists)
            {
                mediaFolder.Create();
            }

            InitializeLocalizableStrings();

            var dataSettings = EngineContext.Current.Resolve<DataSettings>();

            if (dataSettings.CreateSampleData)
            {
                var seeders = EngineContext.Current.ResolveAll<IDbSeeder>().OrderBy(x => x.Order);

                foreach (var seeder in seeders)
                {
                    seeder.Seed(this);
                }
            }
        }

        #endregion ISupportSeed Members

        private void InitializeLocalizableStrings()
        {
            // We need to create localizable strings for all tenants,
            //  but at this point there will only be 1 tenant, because this is initialization for the DB.
            //  TODO: When admin user creates a new tenant, we need to insert localized strings for it. Probably in TenantApiController somewhere...
            int tenantId = Tenants.First().Id;
            var languagePacks = EngineContext.Current.ResolveAll<ILanguagePack>();

            var toInsert = new HashSet<LocalizableString>();
            foreach (var languagePack in languagePacks)
            {
                foreach (var localizedString in languagePack.LocalizedStrings)
                {
                    toInsert.Add(new LocalizableString
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        CultureCode = languagePack.CultureCode,
                        TextKey = localizedString.Key,
                        TextValue = localizedString.Value
                    });
                }
            }
            LocalizableStrings.AddRange(toInsert);
            SaveChanges();
        }
    }
}