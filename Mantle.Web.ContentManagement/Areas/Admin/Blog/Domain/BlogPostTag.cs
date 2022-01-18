using System;
using System.Runtime.Serialization;
using Extenso.Data.Entity;
using Mantle.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Domain
{
    public class BlogPostTag : IEntity
    {
        public Guid PostId { get; set; }

        public int TagId { get; set; }

        public BlogPost Post { get; set; }

        public BlogTag Tag { get; set; }

        #region IEntity Members

        [IgnoreDataMember]
        public object[] KeyValues
        {
            get { return new object[] { PostId, TagId }; }
        }

        #endregion IEntity Members
    }

    public class PostTagMap : IEntityTypeConfiguration<BlogPostTag>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<BlogPostTag> builder)
        {
            builder.ToTable(CmsConstants.Tables.BlogPostTags);
            builder.HasKey(x => new { x.PostId, x.TagId });

            builder.HasOne(x => x.Post).WithMany(x => x.Tags).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Tag).WithMany(x => x.Posts).HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.TagId);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}