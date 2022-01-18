using System;
using Mantle.Data.Entity;
using Mantle.Tenants.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Logging.Domain
{
    public class LogEntry : TenantEntity<int>
    {
        public DateTime EventDateTime { get; set; }

        public string EventLevel { get; set; }

        public string UserName { get; set; }

        public string MachineName { get; set; }

        public string EventMessage { get; set; }

        public string ErrorSource { get; set; }

        public string ErrorClass { get; set; }

        public string ErrorMethod { get; set; }

        public string ErrorMessage { get; set; }

        public string InnerErrorMessage { get; set; }
    }

    public class LogEntryMap : IEntityTypeConfiguration<LogEntry>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<LogEntry> builder)
        {
            builder.ToTable("Mantle_Log");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.EventDateTime).IsRequired();
            builder.Property(m => m.EventLevel).IsRequired().HasMaxLength(5).IsUnicode(false);
            builder.Property(m => m.UserName).IsRequired().HasMaxLength(128).IsUnicode(true);
            builder.Property(m => m.MachineName).IsRequired().HasMaxLength(255).IsUnicode(false);
            builder.Property(m => m.EventMessage).IsRequired().IsUnicode(true);
            builder.Property(m => m.ErrorSource).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(m => m.ErrorClass).HasMaxLength(512).IsUnicode(false);
            builder.Property(m => m.ErrorMethod).HasMaxLength(255).IsUnicode(false);
            builder.Property(m => m.ErrorMessage).IsUnicode(true);
            builder.Property(m => m.InnerErrorMessage).IsUnicode(true);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}