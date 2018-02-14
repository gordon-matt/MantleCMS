using Mantle.Tenants.Domain;
using Microsoft.AspNetCore.Identity;

namespace Mantle.Identity.Domain
{
    public abstract class MantleIdentityUser : IdentityUser, ITenantEntity
    {
        public int? TenantId { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }
}