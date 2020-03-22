using System;
using Extenso.Data.Entity;
using Mantle.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Messaging.Data.Domain
{
    public class MessageTemplateVersion : IEntity
    {
        public int Id { get; set; }

        public int MessageTemplateId { get; set; }

        public string CultureCode { get; set; }

        public string Subject { get; set; }

        public string Data { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateModifiedUtc { get; set; }

        public virtual MessageTemplate MessageTemplate { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class MessageTemplateVersionMap : IEntityTypeConfiguration<MessageTemplateVersion>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<MessageTemplateVersion> builder)
        {
            builder.ToTable("Mantle_MessageTemplateVersions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.MessageTemplateId).IsRequired();
            builder.Property(x => x.CultureCode).HasMaxLength(10).IsUnicode(false);
            builder.Property(x => x.Subject).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(x => x.Data).IsUnicode(true);
            builder.Property(x => x.DateCreatedUtc).IsRequired();
            builder.Property(x => x.DateModifiedUtc).IsRequired();

            builder.HasOne(x => x.MessageTemplate).WithMany().HasForeignKey(x => x.MessageTemplateId);

            builder.HasIndex(x => x.MessageTemplateId);
        }

        public bool IsEnabled => true;
    }
}