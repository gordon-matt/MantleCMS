using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Messaging.Forums.Data.Domain;

public class ForumGroup : TenantEntity<int>
{
    private ICollection<Forum> forums;

    public string Name { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime UpdatedOnUtc { get; set; }

    public virtual ICollection<Forum> Forums
    {
        get { return forums ?? (forums = new List<Forum>()); }
        protected set { forums = value; }
    }
}

public class ForumGroupMap : IEntityTypeConfiguration<ForumGroup>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<ForumGroup> builder)
    {
        builder.ToTable(Constants.Tables.Groups);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.DisplayOrder).IsRequired();
        builder.Property(x => x.CreatedOnUtc).IsRequired();
        builder.Property(x => x.UpdatedOnUtc).IsRequired();
    }

    #region IEntityTypeConfiguration Members

    public bool IsEnabled
    {
        get { return PluginManager.IsPluginInstalled(Constants.PluginSystemName); }
    }

    #endregion IEntityTypeConfiguration Members
}