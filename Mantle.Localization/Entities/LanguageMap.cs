namespace Mantle.Localization.Entities;

public class LanguageMap : IEntityTypeConfiguration<Language>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("Languages", "mantle");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(m => m.CultureCode).IsRequired().HasMaxLength(10).IsUnicode(false);
        builder.Property(m => m.IsRTL).IsRequired();
        builder.Property(m => m.IsEnabled).IsRequired();
        builder.Property(m => m.SortOrder).IsRequired();
    }

    public bool IsEnabled => true;
}