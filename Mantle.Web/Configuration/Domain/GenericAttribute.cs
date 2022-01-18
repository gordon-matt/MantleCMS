using Extenso.Data.Entity;
using Mantle.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Web.Configuration.Domain
{
    public class GenericAttribute : BaseEntity<int>
    {
        public string EntityType { get; set; }

        public string EntityId { get; set; }

        public string Property { get; set; }

        public string Value { get; set; }
    }

    public class GenericAttributeMap : IEntityTypeConfiguration<GenericAttribute>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<GenericAttribute> builder)
        {
            builder.ToTable("Mantle_GenericAttributes");
            builder.HasKey(m => m.Id);
            builder.Property(x => x.EntityType).IsRequired().HasMaxLength(512).IsUnicode(false);
            builder.Property(x => x.EntityId).IsRequired().HasMaxLength(50).IsUnicode(false);
            builder.Property(m => m.Property).IsRequired().HasMaxLength(128).IsUnicode(false);
            builder.Property(x => x.Value).IsUnicode(true);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}