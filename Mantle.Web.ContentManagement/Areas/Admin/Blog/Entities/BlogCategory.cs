namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Entities;

public class BlogCategory : TenantEntity<int>
{
    private ICollection<BlogPost> posts;

    public string Name { get; set; }

    public string UrlSlug { get; set; }

    public ICollection<BlogPost> Posts
    {
        get => posts ??= new HashSet<BlogPost>(); set => posts = value;
    }
}

public class CategoryMap : IEntityTypeConfiguration<BlogCategory>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<BlogCategory> builder)
    {
        builder.ToTable(CmsConstants.Tables.BlogCategories, MantleWebConstants.DatabaseSchemas.Mantle);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.UrlSlug).IsRequired().HasMaxLength(255).IsUnicode(true);
    }

    public bool IsEnabled => true;
}