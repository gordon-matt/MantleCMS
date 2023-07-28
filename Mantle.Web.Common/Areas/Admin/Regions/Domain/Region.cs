using Mantle.Data.Entity;
using Mantle.Tenants.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mantle.Web.Common.Areas.Admin.Regions.Domain;

public class Region : TenantEntity<int>
{
    public string Name { get; set; }

    /// <summary>
    /// Region Type. 0 = Other, 1 = Continent, 2 = Country, 3 = State, 4 = City
    /// </summary>
    public RegionType RegionType { get; set; }

    public string CountryCode { get; set; }

    public bool HasStates { get; set; }

    public string StateCode { get; set; }

    public int? ParentId { get; set; }

    public short? Order { get; set; }

    public virtual Region Parent { get; set; }

    public virtual ICollection<Region> Children { get; set; }
}

public class RegionMap : IEntityTypeConfiguration<Region>, IMantleEntityTypeConfiguration
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.ToTable(Constants.Tables.Regions);
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(255).IsUnicode(true);
        builder.Property(m => m.RegionType).IsRequired();
        builder.Property(m => m.CountryCode).HasMaxLength(2).IsUnicode(false);//.IsFixedLength();
        builder.Property(m => m.HasStates).IsRequired();
        builder.Property(m => m.StateCode).HasMaxLength(10).IsUnicode(false);
        builder.HasOne(p => p.Parent).WithMany(p => p.Children).HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ParentId);
    }

    #region IEntityTypeConfiguration Members

    public bool IsEnabled
    {
        get { return true; }
    }

    #endregion IEntityTypeConfiguration Members
}