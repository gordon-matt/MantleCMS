using Extenso.Data.Entity;
using Mantle.Data.Entity;
using Mantle.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Widgets.FullCalendar.Data.Entities;

public class CalendarEvent : BaseEntity<int>
{
    public int CalendarId { get; set; }

    public string Name { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public virtual Calendar Calendar { get; set; }
}

public class CalendarEntryMap : IEntityTypeConfiguration<CalendarEvent>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.ToTable(Constants.Tables.Events, MantleWebConstants.DatabaseSchemas.Plugins);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CalendarId).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.StartDateTime).IsRequired();
        builder.Property(x => x.EndDateTime).IsRequired();

        builder.HasOne(x => x.Calendar).WithMany(x => x.Events).HasForeignKey(x => x.CalendarId);
    }

    public bool IsEnabled => PluginManager.IsPluginInstalled(Constants.PluginSystemName);
}