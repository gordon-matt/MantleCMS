using System;
using Mantle.Data;
using Mantle.Data.Entity.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Tasks.Domain
{
    public class ScheduledTask : IEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of appropriate ITask class
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the run period (in seconds)
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a task is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether a task should be stopped on some error
        /// </summary>
        public bool StopOnError { get; set; }

        public DateTime? LastStartUtc { get; set; }

        public DateTime? LastEndUtc { get; set; }

        public DateTime? LastSuccessUtc { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }
    public class ScheduledTaskMap : IEntityTypeConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<ScheduledTask>();
            builder.ToTable("Mantle_ScheduledTasks");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(s => s.Type).IsRequired().HasMaxLength(255).IsUnicode(false);
            builder.Property(s => s.Seconds).IsRequired();
            builder.Property(s => s.Enabled).IsRequired();
            builder.Property(s => s.StopOnError).IsRequired();
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}