using Mantle.Data.Entity;
using Mantle.Tenants.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Messaging.Data.Domain
{
    public class MessageTemplate : TenantEntity<int>
    {
        public string Name { get; set; }

        public string Editor { get; set; }

        public Guid? OwnerId { get; set; }

        public bool Enabled { get; set; }
    }

    public class MessageTemplateMap : IEntityTypeConfiguration<MessageTemplate>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<MessageTemplate> builder)
        {
            builder.ToTable("Mantle_MessageTemplates");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(x => x.Editor).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(x => x.Enabled).IsRequired();
        }

        public bool IsEnabled => true;
    }
}