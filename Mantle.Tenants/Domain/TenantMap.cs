using Mantle.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Tenants.Domain;

public class TenantMap : IEntityTypeConfiguration<Tenant>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Mantle_Tenants");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.Url).IsRequired().HasMaxLength(255).IsUnicode(true);
        //builder.Property(x => x.SecureUrl).HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.Hosts).HasMaxLength(1024).IsUnicode(true);
    }

    public bool IsEnabled => true;
}