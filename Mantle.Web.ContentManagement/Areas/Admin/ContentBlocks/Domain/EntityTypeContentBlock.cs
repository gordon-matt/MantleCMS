using System;
using Extenso.Data.Entity;
using Mantle.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Domain
{
    public class EntityTypeContentBlock : BaseEntity<Guid>
    {
        public string EntityType { get; set; }

        public string EntityId { get; set; }

        public string BlockName { get; set; }

        public string BlockType { get; set; }

        public string Title { get; set; }

        public Guid ZoneId { get; set; }

        public int Order { get; set; }

        public bool IsEnabled { get; set; }

        public string BlockValues { get; set; }

        public string CustomTemplatePath { get; set; }
    }

    public class EntityTypeContentBlockMap : IEntityTypeConfiguration<EntityTypeContentBlock>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<EntityTypeContentBlock> builder)
        {
            builder.ToTable(CmsConstants.Tables.EntityTypeContentBlocks);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.EntityType).IsRequired().HasMaxLength(512).IsUnicode(false);
            builder.Property(x => x.EntityId).IsRequired().HasMaxLength(50).IsUnicode(false);
            builder.Property(x => x.BlockName).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(x => x.BlockType).IsRequired().HasMaxLength(1024).IsUnicode(false);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(x => x.ZoneId).IsRequired();
            builder.Property(x => x.Order).IsRequired();
            builder.Property(x => x.IsEnabled).IsRequired();
            builder.Property(x => x.BlockValues).IsUnicode(true);
            builder.Property(x => x.CustomTemplatePath).HasMaxLength(255).IsUnicode(true);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}