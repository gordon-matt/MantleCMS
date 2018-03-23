using System;
using Mantle.Data;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Messaging.Forums.Data.Domain
{
    public class Forum : IEntity
    {
        public int Id { get; set; }

        public int ForumGroupId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int NumTopics { get; set; }

        public int NumPosts { get; set; }

        public int LastTopicId { get; set; }

        public int LastPostId { get; set; }

        public string LastPostUserId { get; set; }

        public DateTime? LastPostTime { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdatedOnUtc { get; set; }

        public virtual ForumGroup ForumGroup { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class ForumMap : IEntityTypeConfiguration<Forum>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<Forum> builder)
        {
            builder.ToTable(Constants.Tables.Forums);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ForumGroupId).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(x => x.Description).IsUnicode(true);
            builder.Property(x => x.NumTopics).IsRequired();
            builder.Property(x => x.NumPosts).IsRequired();
            builder.Property(x => x.LastTopicId).IsRequired();
            builder.Property(x => x.LastPostId).IsRequired();
            builder.Property(x => x.LastPostUserId).HasMaxLength(128).IsUnicode(true);
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.Property(x => x.CreatedOnUtc).IsRequired();
            builder.Property(x => x.UpdatedOnUtc).IsRequired();

            builder.HasOne(x => x.ForumGroup).WithMany(x => x.Forums).HasForeignKey(x => x.ForumGroupId);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return PluginManager.IsPluginInstalled(Constants.PluginSystemName); }
        }

        #endregion IEntityTypeConfiguration Members
    }
}