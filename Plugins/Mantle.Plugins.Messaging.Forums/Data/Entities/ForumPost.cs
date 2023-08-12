using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Messaging.Forums.Data.Entities;

public class ForumPost : BaseEntity<int>
{
    public int TopicId { get; set; }

    public string UserId { get; set; }

    public string Text { get; set; }

    public string IPAddress { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime UpdatedOnUtc { get; set; }

    public virtual ForumTopic ForumTopic { get; set; }
}

public class ForumPostMap : IEntityTypeConfiguration<ForumPost>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<ForumPost> builder)
    {
        builder.ToTable(Constants.Tables.Posts);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TopicId).IsRequired();
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(128).IsUnicode(true);
        builder.Property(x => x.Text).IsRequired().IsUnicode(true);
        builder.Property(x => x.IPAddress).HasMaxLength(45).IsUnicode(false);
        builder.Property(x => x.CreatedOnUtc).IsRequired();
        builder.Property(x => x.UpdatedOnUtc).IsRequired();

        builder.HasOne(x => x.ForumTopic).WithMany().HasForeignKey(x => x.TopicId);

        builder.HasIndex(x => x.TopicId);
    }

    #region IEntityTypeConfiguration Members

    public bool IsEnabled
    {
        get { return PluginManager.IsPluginInstalled(Constants.PluginSystemName); }
    }

    #endregion IEntityTypeConfiguration Members
}