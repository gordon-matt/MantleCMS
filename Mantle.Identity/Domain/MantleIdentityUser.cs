using System.Runtime.Serialization;
using Mantle.Tenants.Domain;
using Microsoft.AspNetCore.Identity;

namespace Mantle.Identity.Domain
{
    public abstract class MantleIdentityUser : IdentityUser, ITenantEntity
    {
        public int? TenantId { get; set; }

        #region IEntity Members

        [IgnoreDataMember] // OData v8 does not like this property and will break if we don't use [IgnoreDataMember] here.
        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }
}