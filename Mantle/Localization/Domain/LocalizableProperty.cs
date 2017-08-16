using System.Runtime.Serialization;
using Mantle.Data;
using Mantle.Data.Entity.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Localization.Domain
{
    [DataContract]
    public class LocalizableProperty : IEntity
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string CultureCode { get; set; }

        [DataMember]
        public string EntityType { get; set; }

        [DataMember]
        public string EntityId { get; set; }

        [DataMember]
        public string Property { get; set; }

        [DataMember]
        public string Value { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class LocalizablePropertyMap : IEntityTypeConfiguration<LocalizableProperty>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<LocalizableProperty> builder)
        {
            builder.ToTable("Mantle_LocalizableProperties");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.CultureCode).HasMaxLength(10).IsUnicode(false);
            builder.Property(x => x.EntityType).IsRequired().HasMaxLength(512).IsUnicode(false);
            builder.Property(x => x.EntityId).IsRequired().HasMaxLength(50).IsUnicode(false);
            builder.Property(m => m.Property).IsRequired().HasMaxLength(128).IsUnicode(false);
            builder.Property(m => m.Value).IsUnicode(true);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}