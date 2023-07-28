namespace Mantle.Messaging.Data;

public interface IMantleMessagingDbContext
{
    DbSet<MessageTemplate> MessageTemplates { get; set; }

    DbSet<QueuedEmail> QueuedEmails { get; set; }
}