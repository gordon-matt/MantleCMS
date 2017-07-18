﻿using System;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Tenants.Domain;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace Mantle.Messaging.Domain
{
    public class QueuedEmail : ITenantEntity, IMailMessage
    {
        public Guid Id { get; set; }

        public int? TenantId { get; set; }

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

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members

        #region IMailMessage Members

        public MimeMessage GetMailMessage()
        {
            var message = new MimeMessage
            {
                Subject = Subject,
                Body = new TextPart("html") { Text = MailMessage }
            };

            message.From.Add(new MailboxAddress(FromName, FromAddress));
            message.To.Add(new MailboxAddress(ToName, ToAddress));
            return message;
        }

        #endregion
    }

    public class QueuedEmailMap : IEntityTypeConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<QueuedEmail>();
            builder.ToTable("Mantle_QueuedEmails");
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
}