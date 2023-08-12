using Mantle.Data.Entity;
using Mantle.Tenants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Widgets.FullCalendar.Data.Entities
{
    public class Calendar : TenantEntity<int>
    {
        private ICollection<CalendarEvent> events;

        public string Name { get; set; }

        public virtual ICollection<CalendarEvent> Events
        {
            get { return events ?? (events = new HashSet<CalendarEvent>()); }
            set { events = value; }
        }
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