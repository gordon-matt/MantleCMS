using System.Collections.Generic;
using Mantle.Data.Entity;
using Mantle.Tenants.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Domain
{
    public class BlogTag : ITenantEntity
    {
        private ICollection<BlogPostTag> posts;

        public int Id { get; set; }

        public int? TenantId { get; set; }

        public string Name { get; set; }

        public string UrlSlug { get; set; }

        public ICollection<BlogPostTag> Posts
        {
            get { return posts ?? (posts = new HashSet<BlogPostTag>()); }
            set { posts = value; }
        }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class TagMap : IEntityTypeConfiguration<BlogTag>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<BlogTag> builder)
        {
            builder.ToTable(CmsConstants.Tables.BlogTags);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(x => x.UrlSlug).IsRequired().HasMaxLength(255).IsUnicode(true);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}