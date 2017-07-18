using System;
using Mantle.Tenants.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Mantle.Identity.Domain
{
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

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }
}