namespace Mantle.Identity.Entities;

public abstract class MantleIdentityRole : IdentityRole, ITenantEntity
{
    public MantleIdentityRole()
        : base()
    {
    }

    public MantleIdentityRole(string roleName)
        : base(roleName)
    {
    }

    public int? TenantId { get; set; }

    #region IEntity Members

    [IgnoreDataMember] // OData v8 does not like this property and will break if we don't use [IgnoreDataMember] here.
    public object[] KeyValues
    {
        get { return new object[] { Id }; }
    }

    #endregion IEntity Members
}