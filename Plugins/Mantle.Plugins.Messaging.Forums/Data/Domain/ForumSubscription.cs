using System;
using Extenso.Data.Entity;
using Mantle.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Messaging.Forums.Data.Domain
{
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
            builder.ToTable(Constants.Tables.Subscriptions);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(128).IsUnicode(true);
            builder.Property(x => x.ForumId).IsRequired();
            builder.Property(x => x.TopicId).IsRequired();
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return PluginManager.IsPluginInstalled(Constants.PluginSystemName); }
        }

        #endregion IEntityTypeConfiguration Members
    }
}