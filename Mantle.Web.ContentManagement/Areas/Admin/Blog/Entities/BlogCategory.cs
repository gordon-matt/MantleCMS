namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Entities;

public class BlogCategory : TenantEntity<int>
{
    private ICollection<BlogPost> posts;

    public string Name { get; set; }

    public string UrlSlug { get; set; }

    public ICollection<BlogPost> Posts
    {
        get { return posts ??= new HashSet<BlogPost>(); }
        set { posts = value; }
    }
}

public class CategoryMap : IEntityTypeConfiguration<BlogCategory>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<BlogCategory> builder)
    {
        builder.ToTable(CmsConstants.Tables.BlogCategories);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.UrlSlug).IsRequired().HasMaxLength(255).IsUnicode(true);
    }

    #region IEntityTypeConfiguration Members

    public bool IsEnabled
    {
        get { return true; }
    }

    #endregion IEntityTypeConfiguration Members
}