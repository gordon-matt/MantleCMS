﻿namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Entities;

public class BlogPost : TenantEntity<Guid>
{
    private ICollection<BlogPostTag> tags;

    public string UserId { get; set; }

    public DateTime DateCreatedUtc { get; set; }

    public int CategoryId { get; set; }

    public string Headline { get; set; }

    public string Slug { get; set; }

    public string TeaserImageUrl { get; set; }

    public string ShortDescription { get; set; }

    public string FullDescription { get; set; }

    public bool UseExternalLink { get; set; }

    public string ExternalLink { get; set; }

    public string MetaKeywords { get; set; }

    public string MetaDescription { get; set; }

    public virtual BlogCategory Category { get; set; }

    public virtual ICollection<BlogPostTag> Tags
    {
        get => tags ??= new HashSet<BlogPostTag>(); set => tags = value;
    }
}

public class PostMap : IEntityTypeConfiguration<BlogPost>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.ToTable(CmsConstants.Tables.BlogPosts, MantleWebConstants.DatabaseSchemas.Mantle);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(128).IsUnicode(true);
        builder.Property(x => x.DateCreatedUtc).IsRequired();
        builder.Property(x => x.CategoryId).IsRequired();
        builder.Property(x => x.Headline).IsRequired().HasMaxLength(128).IsUnicode(true);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(128).IsUnicode(true);
        builder.Property(x => x.TeaserImageUrl).HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.ShortDescription).IsRequired().IsUnicode(true);
        builder.Property(x => x.FullDescription).IsUnicode(true);
        builder.Property(x => x.UseExternalLink).IsRequired();
        builder.Property(x => x.ExternalLink).HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.MetaKeywords).HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.MetaDescription).HasMaxLength(255).IsUnicode(true);

        builder.HasOne(x => x.Category).WithMany(x => x.Posts).HasForeignKey(x => x.CategoryId);
        //HasMany(c => c.Tags).WithMany(x => x.Posts).Map(m =>
        //{
        //    m.MapLeftKey("PostId");
        //    m.MapRightKey("TagId");
        //    m.builder.ToTable("Mantle_BlogPostTags");
        //});

        builder.HasIndex(x => x.CategoryId);
    }

    public bool IsEnabled => true;
}