using System.Runtime.Serialization;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Entities;

public class BlogPostTag : IEntity
{
    public Guid PostId { get; set; }

    public int TagId { get; set; }

    public BlogPost Post { get; set; }

    public BlogTag Tag { get; set; }

    [IgnoreDataMember]
    public object[] KeyValues => new object[] { PostId, TagId };
}

public class PostTagMap : IEntityTypeConfiguration<BlogPostTag>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<BlogPostTag> builder)
    {
        builder.ToTable(CmsConstants.Tables.BlogPostTags, MantleWebConstants.DatabaseSchemas.Mantle);
        builder.HasKey(x => new { x.PostId, x.TagId });

        builder.HasOne(x => x.Post).WithMany(x => x.Tags).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Tag).WithMany(x => x.Posts).HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.TagId);
    }

    public bool IsEnabled => true;
}