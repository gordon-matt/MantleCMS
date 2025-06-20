﻿using System.Runtime.Serialization;

namespace Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Entities;

[DataContract]
public class SitemapConfig : TenantEntity<int>
{
    [DataMember]
    public Guid PageId { get; set; }

    [DataMember]
    public ChangeFrequency ChangeFrequency { get; set; }

    [DataMember]
    public float Priority { get; set; } = .5f;
}

public class SitemapConfigMap : IEntityTypeConfiguration<SitemapConfig>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<SitemapConfig> builder)
    {
        builder.ToTable(CmsConstants.Tables.SitemapConfig, MantleWebConstants.DatabaseSchemas.Mantle);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ChangeFrequency).IsRequired();
        builder.Property(x => x.Priority).IsRequired();
    }

    public bool IsEnabled => true;
}