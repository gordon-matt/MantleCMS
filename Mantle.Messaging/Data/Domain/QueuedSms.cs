//using System;
//using Mantle.Data.Entity;
//using Mantle.Tenants.Domain;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace Mantle.Messaging.Data.Domain
//{
//    public class QueuedSms : ITenantEntity
//    {
//        public Guid Id { get; set; }

//        public int? TenantId { get; set; }

//        /// <summary>
//        /// Gets or sets the priority
//        /// </summary>
//        public int Priority { get; set; }

//        /// <summary>
//        /// Gets or sets the From Number property
//        /// </summary>
//        public string FromNumber { get; set; }

//        /// <summary>
//        /// Gets or sets the To Number property
//        /// </summary>
//        public string ToNumber { get; set; }

//        /// <summary>
//        /// Gets or sets the Message property
//        /// </summary>
//        public string Message { get; set; }

//        /// <summary>
//        /// Gets or sets the date and time of item creation in UTC
//        /// </summary>
//        public DateTime CreatedOnUtc { get; set; }

//        /// <summary>
//        /// Gets or sets the send tries
//        /// </summary>
//        public int SentTries { get; set; }

//        /// <summary>
//        /// Gets or sets the sent date and time
//        /// </summary>
//        public DateTime? SentOnUtc { get; set; }

//        #region IEntity Members

//        public object[] KeyValues
//        {
//            get { return new object[] { Id }; }
//        }

//        #endregion IEntity Members
//    }

//    public class QueuedSmsMap : IEntityTypeConfiguration<QueuedSms>, IMantleEntityTypeConfiguration
//    {
//        public void Configure(EntityTypeBuilder<QueuedSms> builder)
//        {
//            builder.ToTable("Mantle_QueuedSMS");
//            builder.HasKey(x => x.Id);
//            builder.Property(x => x.Priority).IsRequired();
//            builder.Property(x => x.FromNumber).HasMaxLength(30);
//            builder.Property(x => x.ToNumber).HasMaxLength(30).IsRequired();
//            builder.Property(x => x.Message).IsRequired();
//            builder.Property(x => x.CreatedOnUtc).IsRequired();
//            builder.Property(x => x.SentTries).IsRequired();
//        }

//        public bool IsEnabled => true;
//    }
//}