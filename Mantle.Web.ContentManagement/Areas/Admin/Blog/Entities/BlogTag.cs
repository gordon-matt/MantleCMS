namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Entities;

public class BlogTag : TenantEntity<int>
{
    private ICollection<BlogPostTag> posts;

    public string Name { get; set; }

    public string UrlSlug { get; set; }

    public ICollection<BlogPostTag> Posts
    {
        get => posts ??= new HashSet<BlogPostTag>(); set => posts = value;
    }
}

public class TagMap : IEntityTypeConfiguration<BlogTag>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<BlogTag> builder)
    {
        builder.ToTable(CmsConstants.Tables.BlogTags, MantleWebConstants.DatabaseSchemas.Mantle);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.UrlSlug).IsRequired().HasMaxLength(255).IsUnicode(true);
    }

    public bool IsEnabled => true;
}