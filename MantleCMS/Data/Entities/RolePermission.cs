using Mantle.Data.Entity;
using Mantle.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Runtime.Serialization;

namespace MantleCMS.Data.Entities;

public class RolePermission : IEntity
{
    public int PermissionId { get; set; }

    public string RoleId { get; set; }

    #region IEntity Members

    [IgnoreDataMember]
    public object[] KeyValues
    {
        get { return new object[] { Permission, RoleId }; }
    }

    #endregion IEntity Members

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

    #region IEntityTypeConfiguration Members

    public bool IsEnabled
    {
        get { return true; }
    }

    #endregion IEntityTypeConfiguration Members
}