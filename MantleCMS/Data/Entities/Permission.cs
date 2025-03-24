using Mantle.Data.Entity;
using Mantle.Tenants.Entities;
using Mantle.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MantleCMS.Data.Entities;

public class Permission : TenantEntity<int>
{
    public string Name { get; set; }

    public string Category { get; set; }

    public string Description { get; set; }

    public virtual ICollection<RolePermission> RolesPermissions { get; set; }
}

public class PermissionMap : IEntityTypeConfiguration<Permission>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(Constants.Tables.Permissions, MantleWebConstants.DatabaseSchemas.Mantle);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50).IsUnicode(true);
        builder.Property(x => x.Category).IsRequired().HasMaxLength(50).IsUnicode(true);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(128).IsUnicode(true);
    }

    public bool IsEnabled => true;
}