﻿using System;
using Mantle.Data;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Web.Plugins;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Plugins.Messaging.Forums.Data.Domain
{
    public class ForumSubscription : IEntity
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ForumId { get; set; }

        public int TopicId { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class ForumSubscriptionMap : IEntityTypeConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<ForumSubscription>();
            builder.ToTable(Constants.Tables.Subscriptions);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(128).IsUnicode(true);
            builder.Property(x => x.ForumId).IsRequired();
            builder.Property(x => x.TopicId).IsRequired();
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return PluginManager.IsPluginInstalled(Constants.PluginSystemName); }
        }

        #endregion IEntityTypeConfiguration Members
    }
}