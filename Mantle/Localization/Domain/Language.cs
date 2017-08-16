using System;
using System.Runtime.Serialization;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Tenants.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Localization.Domain
{
    [DataContract]
    public class Language : ITenantEntity
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public int? TenantId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string CultureCode { get; set; }

        [DataMember]
        public bool IsRTL { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public int SortOrder { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class LanguageMap : IEntityTypeConfiguration<Language>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Mantle_Languages");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(m => m.CultureCode).IsRequired().HasMaxLength(10).IsUnicode(false);
            builder.Property(m => m.IsRTL).IsRequired();
            builder.Property(m => m.IsEnabled).IsRequired();
            builder.Property(m => m.SortOrder).IsRequired();
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}