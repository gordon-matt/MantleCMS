﻿namespace Mantle.Localization.Entities;

public class LocalizableStringMap : IEntityTypeConfiguration<LocalizableString>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<LocalizableString> builder)
    {
        builder.ToTable("LocalizableStrings", "mantle");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.CultureCode).HasMaxLength(10).IsUnicode(false);
        builder.Property(m => m.TextKey).IsRequired().IsUnicode(true);
        builder.Property(m => m.TextValue).IsUnicode(true);
    }

    #region IEntityTypeConfiguration Members

    public bool IsEnabled
    {
        get { return true; }
    }

    #endregion IEntityTypeConfiguration Members
}