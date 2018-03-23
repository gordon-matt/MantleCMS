using System.Collections.Generic;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Tenants.Domain;
using Mantle.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Widgets.FullCalendar.Data.Domain
{
    public class Calendar : ITenantEntity
    {
        private ICollection<CalendarEvent> events;

        public int Id { get; set; }

        public int? TenantId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CalendarEvent> Events
        {
            get { return events ?? (events = new HashSet<CalendarEvent>()); }
            set { events = value; }
        }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class CalendarMap : IEntityTypeConfiguration<Calendar>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<Calendar> builder)
        {
            builder.ToTable(Constants.Tables.Calendars);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return PluginManager.IsPluginInstalled(Constants.PluginSystemName); }
        }

        #endregion IEntityTypeConfiguration Members
    }
}