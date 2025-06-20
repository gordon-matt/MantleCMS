using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Messaging.Forums.Data.Entities;

public class ForumTopic : BaseEntity<int>
{
    public int ForumId { get; set; }

    public string UserId { get; set; }

    public ForumTopicType TopicType { get; set; }

    public string Subject { get; set; }

    public int NumPosts { get; set; }

    public int Views { get; set; }

    public int LastPostId { get; set; }

    public string LastPostUserId { get; set; }

    public DateTime? LastPostTime { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime UpdatedOnUtc { get; set; }

    public virtual Forum Forum { get; set; }

    public int NumReplies => NumPosts > 0 ? (NumPosts - 1) : 0;
}

public class ForumTopicMap : IEntityTypeConfiguration<ForumTopic>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<ForumTopic> builder)
    {
        builder.ToTable(Constants.Tables.Topics, MantleWebConstants.DatabaseSchemas.Plugins);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ForumId).IsRequired();
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(128).IsUnicode(true);
        builder.Property(x => x.TopicType).IsRequired();
        builder.Property(x => x.Subject).IsRequired().HasMaxLength(512).IsUnicode(true);
        builder.Property(x => x.NumPosts).IsRequired();
        builder.Property(x => x.Views).IsRequired();
        builder.Property(x => x.LastPostId).IsRequired();
        builder.Property(x => x.LastPostUserId).HasMaxLength(128).IsUnicode(true);
        builder.Property(x => x.CreatedOnUtc).IsRequired();
        builder.Property(x => x.UpdatedOnUtc).IsRequired();

        builder.HasOne(x => x.Forum).WithMany().HasForeignKey(x => x.ForumId);

        builder.HasIndex(x => x.ForumId);
    }

    public bool IsEnabled => PluginManager.IsPluginInstalled(Constants.PluginSystemName);
}