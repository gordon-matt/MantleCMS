using Mantle.Data.Entity.EntityFramework;
using Mantle.Tenants.Domain;
using Microsoft.EntityFrameworkCore;

namespace MantleCMS.Data.Domain
{
    public class UserProfileEntry : ITenantEntity
    {
        public int Id { get; set; }

        public int? TenantId { get; set; }

        public string UserId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class UserProfileEntryMap : IEntityTypeConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<UserProfileEntry>();
            builder.ToTable(Constants.Tables.UserProfiles);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(128).IsUnicode(false);
            builder.Property(x => x.Key).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(x => x.Value).IsRequired().IsUnicode(true);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}