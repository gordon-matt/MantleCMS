using System;
using Mantle.Data;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Web.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Messaging.Forums.Data.Domain
{
    public class ForumPost : IEntity
    {
        public int Id { get; set; }

        public int TopicId { get; set; }

        public string UserId { get; set; }

        public string Text { get; set; }

        public string IPAddress { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdatedOnUtc { get; set; }

        public virtual ForumTopic ForumTopic { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
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
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return PluginManager.IsPluginInstalled(Constants.PluginSystemName); }
        }

        #endregion IEntityTypeConfiguration Members
    }
}