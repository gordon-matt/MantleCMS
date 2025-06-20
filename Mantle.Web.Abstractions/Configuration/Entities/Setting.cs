﻿using System.Diagnostics;
using Mantle.Data.Entity;
using Mantle.Tenants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Web.Configuration.Entities;

[DebuggerDisplay("{Name}")]
public class Setting : TenantEntity<Guid>
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the value
    /// </summary>
    public string Value { get; set; }
}

public class SettingMap : IEntityTypeConfiguration<Setting>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("Settings", MantleWebConstants.DatabaseSchemas.Mantle);
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(s => s.Type).IsRequired().HasMaxLength(255).IsUnicode(false);
        builder.Property(s => s.Value).IsUnicode(true);
    }

    public bool IsEnabled => true;
}