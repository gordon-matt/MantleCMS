using System;
using System.Runtime.Serialization;
using Mantle.Data.Entity;
using Mantle.Tenants.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Domain
{
    [DataContract]
    public class SitemapConfig : ITenantEntity
    {
        public SitemapConfig()
        {
            Priority = .5f;
        }

        [DataMember]
        public int Id { get; set; }

        [IgnoreDataMember]
        public int? TenantId { get; set; }

        [DataMember]
        public Guid PageId { get; set; }

        [DataMember]
        public ChangeFrequency ChangeFrequency { get; set; }

        [DataMember]
        public float Priority { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class SitemapConfigMap : IEntityTypeConfiguration<SitemapConfig>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<SitemapConfig> builder)
        {
            builder.ToTable(CmsConstants.Tables.SitemapConfig);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ChangeFrequency).IsRequired();
            builder.Property(x => x.Priority).IsRequired();
        }

        public bool IsEnabled => true;
    }
}