﻿namespace Mantle.Web.ContentManagement.Areas.Admin.Pages.Entities;

public class PageVersion : TenantEntity<Guid>
{
    public Guid PageId { get; set; }

    public string CultureCode { get; set; }

    public DateTime DateCreatedUtc { get; set; }

    public DateTime DateModifiedUtc { get; set; }

    public VersionStatus Status { get; set; }

    public string Title { get; set; }

    public string Slug { get; set; }

    public string Fields { get; set; }

    public virtual Page Page { get; set; }
}

public enum VersionStatus : byte
{
    Draft = 0,
    Published = 1,
    Archived = 2
}

public class PageVersionMap : IEntityTypeConfiguration<PageVersion>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<PageVersion> builder)
    {
        builder.ToTable(CmsConstants.Tables.PageVersions, MantleWebConstants.DatabaseSchemas.Mantle);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PageId).IsRequired();
        builder.Property(x => x.CultureCode).HasMaxLength(10).IsUnicode(false);
        builder.Property(x => x.DateCreatedUtc).IsRequired();
        builder.Property(x => x.DateModifiedUtc).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Title).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.Fields).IsUnicode(true);

        builder.HasOne(x => x.Page).WithMany(x => x.Versions).HasForeignKey(x => x.PageId);

        builder.HasIndex(x => x.PageId);
    }

    public bool IsEnabled => true;
}