﻿using System;
using Extenso.Data.Entity;
using Mantle.Data;
using Mantle.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Plugins.Widgets.FullCalendar.Data.Domain
{
    public class CalendarEvent : IEntity
    {
        public int Id { get; set; }

        public int CalendarId { get; set; }

        public string Name { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public virtual Calendar Calendar { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class CalendarEntryMap : IEntityTypeConfiguration<CalendarEvent>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<CalendarEvent> builder)
        {
            builder.ToTable(Constants.Tables.Events);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CalendarId).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(x => x.StartDateTime).IsRequired();
            builder.Property(x => x.EndDateTime).IsRequired();

            builder.HasOne(x => x.Calendar).WithMany(x => x.Events).HasForeignKey(x => x.CalendarId);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return PluginManager.IsPluginInstalled(Constants.PluginSystemName); }
        }

        #endregion IEntityTypeConfiguration Members
    }
}