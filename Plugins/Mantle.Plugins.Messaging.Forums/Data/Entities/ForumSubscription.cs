using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Messaging.Forums.Data.Entities;

public class ForumSubscription : BaseEntity<int>
{
    public string UserId { get; set; }

    public int ForumId { get; set; }

    public int TopicId { get; set; }

    public DateTime CreatedOnUtc { get; set; }
}

public class ForumSubscriptionMap : IEntityTypeConfiguration<ForumSubscription>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<ForumSubscription> builder)
    {
        builder.ToTable(Constants.Tables.Subscriptions, MantleWebConstants.DatabaseSchemas.Plugins);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(128).IsUnicode(true);
        builder.Property(x => x.ForumId).IsRequired();
        builder.Property(x => x.TopicId).IsRequired();
    }

    public bool IsEnabled => PluginManager.IsPluginInstalled(Constants.PluginSystemName);
}