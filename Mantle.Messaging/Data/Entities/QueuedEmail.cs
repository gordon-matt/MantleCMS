﻿namespace Mantle.Messaging.Data.Entities;

public class QueuedEmail : TenantEntity<Guid>, IMailMessage
{
    /// <summary>
    /// Gets or sets the priority
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets the From Address property
    /// </summary>
    public string FromAddress { get; set; }

    /// <summary>
    /// Gets or sets the FromName property
    /// </summary>
    public string FromName { get; set; }

    /// <summary>
    /// Gets or sets the To property
    /// </summary>
    public string ToAddress { get; set; }

    /// <summary>
    /// Gets or sets the ToName property
    /// </summary>
    public string ToName { get; set; }

    /// <summary>
    /// Gets or sets the subject
    /// </summary>
    public string Subject { get; set; }

    public string MailMessage { get; set; }

    /// <summary>
    /// Gets or sets the date and time of item creation in UTC
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the send tries
    /// </summary>
    public int SentTries { get; set; }

    /// <summary>
    /// Gets or sets the sent date and time
    /// </summary>
    public DateTime? SentOnUtc { get; set; }

    #region IMailMessage Members

    public MailMessage GetMailMessage()
    {
        var wrap = MailMessageWrapper.Create(MailMessage);
        return wrap.ToMailMessage();
    }

    #endregion IMailMessage Members
}

public class QueuedEmailMap : IEntityTypeConfiguration<QueuedEmail>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<QueuedEmail> builder)
    {
        builder.ToTable("QueuedEmails", "mantle");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Priority).IsRequired();
        builder.Property(x => x.FromAddress).HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.FromName).HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.ToAddress).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.ToName).HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.Subject).HasMaxLength(255).IsUnicode(true);
        builder.Property(x => x.MailMessage).IsRequired().IsUnicode(true);
        builder.Property(x => x.CreatedOnUtc).IsRequired();
        builder.Property(x => x.SentTries).IsRequired();
    }

    public bool IsEnabled => true;
}