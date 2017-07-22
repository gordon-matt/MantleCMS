using Mantle.Data;
using Mantle.Data.Entity.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Web.Common.Areas.Admin.Regions.Domain
{
    public class RegionSettings : IEntity
    {
        public int Id { get; set; }

        public int RegionId { get; set; }

        public string SettingsId { get; set; }

        public string Fields { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class RegionSettingsMap : IEntityTypeConfiguration
    {
        #region IEntityTypeConfiguration Members

        public void Configure(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<RegionSettings>();
            builder.ToTable(Constants.Tables.RegionSettings);
            builder.HasKey(m => m.Id);
            builder.Property(m => m.RegionId).IsRequired();
            builder.Property(m => m.SettingsId).IsRequired().HasMaxLength(255).IsUnicode(false);
            builder.Property(m => m.Fields).IsRequired().IsUnicode(true);
        }

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}