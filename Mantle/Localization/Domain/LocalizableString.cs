using System;
using System.Runtime.Serialization;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Tenants.Domain;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Localization.Domain
{
    [DataContract]
    public class LocalizableString : ITenantEntity
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public int? TenantId { get; set; }

        [DataMember]
        public string CultureCode { get; set; }

        [DataMember]
        public string TextKey { get; set; }

        [DataMember]
        public string TextValue { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class LocalizableStringMap : IEntityTypeConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<LocalizableString>();
            builder.ToTable("Mantle_LocalizableStrings");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.CultureCode).HasMaxLength(10).IsUnicode(false);
            builder.Property(m => m.TextKey).IsRequired().IsUnicode(true);
            builder.Property(m => m.TextValue).IsUnicode(true);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}