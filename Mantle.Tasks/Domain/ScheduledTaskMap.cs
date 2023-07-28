namespace Mantle.Tasks.Domain;

public class ScheduledTaskMap : IEntityTypeConfiguration<ScheduledTask>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<ScheduledTask> builder)
    {
        builder.ToTable("Mantle_ScheduledTasks");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(s => s.Type).IsRequired().HasMaxLength(255).IsUnicode(false);
        builder.Property(s => s.Seconds).IsRequired();
        builder.Property(s => s.Enabled).IsRequired();
        builder.Property(s => s.StopOnError).IsRequired();
    }

    public bool IsEnabled => true;
}