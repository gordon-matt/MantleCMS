using Mantle.Data.Entity;
using Mantle.Tenants.Entities;
using Mantle.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MantleCMS.Data.Entities;

public class UserProfileEntry : TenantEntity<int>
{
    public string UserId { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }
}

public class UserProfileEntryMap : IEntityTypeConfiguration<UserProfileEntry>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<UserProfileEntry> builder)
    {
        builder.ToTable(Constants.Tables.UserProfiles, MantleWebConstants.DatabaseSchemas.Mantle);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(128).IsUnicode(false);
        builder.Property(x => x.Key).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.Value).IsRequired().IsUnicode(true);
    }

    public bool IsEnabled => true;
}