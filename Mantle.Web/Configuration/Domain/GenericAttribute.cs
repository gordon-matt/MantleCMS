using Mantle.Data;
using Mantle.Data.Entity.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Web.Configuration.Domain
{
    public class GenericAttribute : IEntity
    {
        public int Id { get; set; }

        public string EntityType { get; set; }

        public string EntityId { get; set; }

        public string Property { get; set; }

        public string Value { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class GenericAttributeMap : IEntityTypeConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<GenericAttribute>();
            builder.ToTable("Mantle_GenericAttributes");
            builder.HasKey(m => m.Id);
            builder.Property(x => x.EntityType).IsRequired().HasMaxLength(512).IsUnicode(false);
            builder.Property(x => x.EntityId).IsRequired().HasMaxLength(50).IsUnicode(false);
            builder.Property(m => m.Property).IsRequired().HasMaxLength(128).IsUnicode(false);
            builder.Property(x => x.Value).IsUnicode(true);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}