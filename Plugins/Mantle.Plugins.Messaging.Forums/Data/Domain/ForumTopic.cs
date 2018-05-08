using System;
using Mantle.Data;
using Mantle.Data.Entity;
using Mantle.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Messaging.Forums.Data.Domain
{
    public class ForumTopic : IEntity
    {
        public int Id { get; set; }

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

        public int NumReplies
        {
            get { return NumPosts > 0 ? (NumPosts - 1) : 0; }
        }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class ForumTopicMap : IEntityTypeConfiguration<ForumTopic>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<ForumTopic> builder)
        {
            builder.ToTable(Constants.Tables.Topics);
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
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return PluginManager.IsPluginInstalled(Constants.PluginSystemName); }
        }

        #endregion IEntityTypeConfiguration Members
    }
}