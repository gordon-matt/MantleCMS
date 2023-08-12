namespace Mantle.Localization.Entities;

public class LocalizablePropertyMap : IEntityTypeConfiguration<LocalizableProperty>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<LocalizableProperty> builder)
    {
        builder.ToTable("Mantle_LocalizableProperties");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.CultureCode).HasMaxLength(10).IsUnicode(false);
        builder.Property(x => x.EntityType).IsRequired().HasMaxLength(512).IsUnicode(false);
        builder.Property(x => x.EntityId).IsRequired().HasMaxLength(50).IsUnicode(false);
        builder.Property(m => m.Property).IsRequired().HasMaxLength(128).IsUnicode(false);
        builder.Property(m => m.Value).IsUnicode(true);
    }

    #region IEntityTypeConfiguration Members

    public bool IsEnabled
    {
        get { return true; }
    }

    #endregion IEntityTypeConfiguration Members
}