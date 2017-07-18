using System;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Tenants.Domain;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Messaging.Domain
{
    public class MessageTemplate : ITenantEntity
    {
        public Guid Id { get; set; }

        public int? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the owner of template
        /// </summary>
        public Guid? OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the template is enable
        /// </summary>
        public bool Enabled { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class MessageTemplateMap : IEntityTypeConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<MessageTemplate>();
            builder.ToTable("Mantle_MessageTemplates");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(x => x.Subject).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(x => x.Body).IsRequired().IsUnicode(true);
            builder.Property(x => x.Enabled).IsRequired();
        }

        public bool IsEnabled => true;
    }
}