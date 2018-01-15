using System;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Tenants.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Domain
{
    public class Zone : ITenantEntity
    {
        public Guid Id { get; set; }

        public int? TenantId { get; set; }

        public string Name { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class ZoneMap : IEntityTypeConfiguration<Zone>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<Zone> builder)
        {
            builder.ToTable(CmsConstants.Tables.Zones);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
        }

        public bool IsEnabled => true;
    }
}