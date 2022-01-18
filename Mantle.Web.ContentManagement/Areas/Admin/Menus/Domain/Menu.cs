using System;
using Mantle.Data.Entity;
using Mantle.Tenants.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Web.ContentManagement.Areas.Admin.Menus.Domain
{
    public class Menu : TenantEntity<Guid>
    {
        public string Name { get; set; }

        public string UrlFilter { get; set; }
    }

    public class MenuMap : IEntityTypeConfiguration<Menu>, IMantleEntityTypeConfiguration
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.ToTable(CmsConstants.Tables.Menus);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
            builder.Property(x => x.UrlFilter).HasMaxLength(255).IsUnicode(true);
        }

        public bool IsEnabled => true;
    }
}