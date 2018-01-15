using System;
using Mantle.Data;
using Mantle.Data.Entity.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages.Domain
{
    public class PageType : IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string LayoutPath { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class PageTypeMap : IEntityTypeConfiguration<PageType>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<PageType> builder)
        {
            builder.ToTable(CmsConstants.Tables.PageTypes);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(x => x.LayoutPath).HasMaxLength(255).IsUnicode(true);
        }

        public bool IsEnabled => true;
    }
}