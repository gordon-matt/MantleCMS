namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Domain;

public class Zone : TenantEntity<Guid>
{
    public string Name { get; set; }
}

public class ZoneMap : IEntityTypeConfiguration<Zone>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        builder.ToTable(CmsConstants.Tables.Zones);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
    }

    public bool IsEnabled => true;
}