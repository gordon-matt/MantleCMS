using System;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Tenants.Domain;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Configuration.Domain
{
    public class Setting : ITenantEntity
    {
        public Guid Id { get; set; }

        public int? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members

        public override string ToString()
        {
            return Name;
        }
    }

    public class SettingMap : IEntityTypeConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<Setting>();
            builder.ToTable("Mantle_Settings");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(s => s.Type).IsRequired().HasMaxLength(255).IsUnicode(false);
            builder.Property(s => s.Value).IsUnicode(true);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}