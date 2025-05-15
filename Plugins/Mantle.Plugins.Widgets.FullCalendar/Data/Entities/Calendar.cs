using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Widgets.FullCalendar.Data.Entities;

public class Calendar : TenantEntity<int>
{
    private ICollection<CalendarEvent> events;

    public string Name { get; set; }

    public virtual ICollection<CalendarEvent> Events
    {
        get => events ??= new HashSet<CalendarEvent>(); set => events = value;
    }
}

public class CalendarMap : IEntityTypeConfiguration<Calendar>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<Calendar> builder)
    {
        builder.ToTable(Constants.Tables.Calendars, MantleWebConstants.DatabaseSchemas.Plugins);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
    }

    public bool IsEnabled => PluginManager.IsPluginInstalled(Constants.PluginSystemName);
}