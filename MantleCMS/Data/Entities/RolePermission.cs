using System.Runtime.Serialization;
using Mantle.Data.Entity;
using Mantle.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MantleCMS.Data.Entities;

public class RolePermission : IEntity
{
    public int PermissionId { get; set; }

    public string RoleId { get; set; }

    [IgnoreDataMember]
    public object[] KeyValues => new object[] { Permission, RoleId };

    public virtual Permission Permission { get; set; }

    public virtual ApplicationRole Role { get; set; }
}

public class RolePermissionMap : IEntityTypeConfiguration<RolePermission>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable(Constants.Tables.RolePermissions, MantleWebConstants.DatabaseSchemas.Mantle);
        builder.HasKey(x => new { x.PermissionId, x.RoleId });
        builder.Property(x => x.PermissionId).IsRequired();
        builder.Property(x => x.RoleId).IsRequired().HasMaxLength(450).IsUnicode(true);

        builder.HasOne(x => x.Permission).WithMany(x => x.RolesPermissions).HasForeignKey(x => x.PermissionId);
        builder.HasOne(x => x.Role).WithMany(x => x.RolesPermissions).HasForeignKey(x => x.RoleId);

        builder.HasIndex(x => x.RoleId);
    }

    public bool IsEnabled => true;
}