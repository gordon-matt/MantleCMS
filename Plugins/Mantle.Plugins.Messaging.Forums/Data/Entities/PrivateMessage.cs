using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Messaging.Forums.Data.Entities;

public class PrivateMessage : BaseEntity<int>
{
    //public int TenantId { get; set; }

    public string FromUserId { get; set; }

    public string ToUserId { get; set; }

    public string Subject { get; set; }

    public string Text { get; set; }

    public bool IsRead { get; set; }

    public bool IsDeletedByAuthor { get; set; }

    public bool IsDeletedByRecipient { get; set; }

    public DateTime CreatedOnUtc { get; set; }
}

public class PrivateMessageMap : IEntityTypeConfiguration<PrivateMessage>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<PrivateMessage> builder)
    {
        builder.ToTable(Constants.Tables.PrivateMessages);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FromUserId).IsRequired().HasMaxLength(128).IsUnicode(true);
        builder.Property(x => x.ToUserId).IsRequired().HasMaxLength(128).IsUnicode(true);
        builder.Property(x => x.Subject).IsRequired().HasMaxLength(512).IsUnicode(true);
        builder.Property(x => x.Text).IsRequired().IsUnicode(true);
        builder.Property(x => x.IsRead).IsRequired();
        builder.Property(x => x.IsDeletedByAuthor).IsRequired();
        builder.Property(x => x.IsDeletedByRecipient).IsRequired();
        builder.Property(x => x.CreatedOnUtc).IsRequired();
    }

    #region IEntityTypeConfiguration Members

    public bool IsEnabled
    {
        get { return PluginManager.IsPluginInstalled(Constants.PluginSystemName); }
    }

    #endregion IEntityTypeConfiguration Members
}