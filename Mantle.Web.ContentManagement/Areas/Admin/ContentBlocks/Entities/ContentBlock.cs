namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Entities;

public class ContentBlock : BaseEntity<Guid>
{
    public string BlockName { get; set; }

    public string BlockType { get; set; }

    public string Title { get; set; }

    public Guid ZoneId { get; set; }

    public int Order { get; set; }

    public bool IsEnabled { get; set; }

    public string BlockValues { get; set; }

    public string CustomTemplatePath { get; set; }

    public Guid? PageId { get; set; }
}

public class ContentBlockMap : IEntityTypeConfiguration<ContentBlock>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<ContentBlock> builder)
    {
        builder.ToTable(CmsConstants.Tables.ContentBlocks, MantleWebConstants.DatabaseSchemas.Mantle);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.BlockName).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.BlockType).IsRequired().HasMaxLength(1024).IsUnicode(false);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.ZoneId).IsRequired();
        builder.Property(x => x.Order).IsRequired();
        builder.Property(x => x.IsEnabled).IsRequired();
        builder.Property(x => x.BlockValues).IsUnicode(true);
        builder.Property(x => x.CustomTemplatePath).HasMaxLength(255).IsUnicode(true);
        //builder.Property(x => x.CultureCode).HasMaxLength(10).HasColumnType("varchar");
    }

    #region IEntityTypeConfiguration Members

    public bool IsEnabled
    {
        get { return true; }
    }

    #endregion IEntityTypeConfiguration Members
}