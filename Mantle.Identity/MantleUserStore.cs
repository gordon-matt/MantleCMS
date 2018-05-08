using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Mantle.Identity.Domain;
using Mantle.Infrastructure;
using Mantle.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Identity
{
    public abstract class MantleUserStore<TUser, TRole, TContext>
        : UserStore<TUser, TRole, TContext, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>
        where TUser : MantleIdentityUser
        where TRole : MantleIdentityRole
        where TContext : DbContext
    {
        private IWorkContext workContext;

        public MantleUserStore(TContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
        }

        #region Private Properties

        private DbSet<TUser> UsersSet
        {
            get { return Context.Set<TUser>(); }
        }

        private DbSet<TRole> Roles
        {
            get { return Context.Set<TRole>(); }
        }

        private DbSet<IdentityUserClaim<string>> UserClaims
        {
            get { return Context.Set<IdentityUserClaim<string>>(); }
        }

        private DbSet<IdentityUserRole<string>> UserRoles
        {
            get { return Context.Set<IdentityUserRole<string>>(); }
        }

        private DbSet<IdentityUserLogin<string>> UserLogins
        {
            get { return Context.Set<IdentityUserLogin<string>>(); }
        }

        private DbSet<IdentityUserToken<string>> UserTokens
        {
            get { return Context.Set<IdentityUserToken<string>>(); }
        }

        private IWorkContext WorkContext
        {
            get
            {
                if (workContext == null)
                {
                    workContext = EngineContext.Current.Resolve<IWorkContext>();
                }
                return workContext;
            }
        }

        private int TenantId
        {
            get { return WorkContext.CurrentTenant.Id; }
        }

        #endregion Private Properties

        protected override IdentityUserRole<string> CreateUserRole(TUser user, TRole role)
        {
            return new IdentityUserRole<string>()
            {
                UserId = user.Id,
                RoleId = role.Id
            };
        }

        protected override IdentityUserClaim<string> CreateUserClaim(TUser user, Claim claim)
        {
            var userClaim = new IdentityUserClaim<string> { UserId = user.Id };
            userClaim.InitializeFromClaim(claim);
            return userClaim;
        }

        protected override IdentityUserLogin<string> CreateUserLogin(TUser user, UserLoginInfo login)
        {
            return new IdentityUserLogin<string>
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };
        }

        protected override IdentityUserToken<string> CreateUserToken(TUser user, string loginProvider, string name, string value)
        {
            return new IdentityUserToken<string>
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };
        }

        public override Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Users.FirstOrDefaultAsync(
                u =>
                    u.NormalizedEmail == normalizedEmail
                    && (u.TenantId == TenantId || (u.TenantId == null)),
                cancellationToken);
        }

        public override Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Users.FirstOrDefaultAsync(
                u =>
                    u.NormalizedUserName == normalizedUserName
                    && (u.TenantId == TenantId || (u.TenantId == null)),
                cancellationToken);
        }

        public override async Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(normalizedRoleName));
            }

            var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (roleEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Role {0} does not exist.", normalizedRoleName));
            }

            UserRoles.Add(CreateUserRole(user, roleEntity));
        }

        public override async Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(normalizedRoleName));
            }

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null)
            {
                var userRole = await FindUserRoleAsync(user.Id, role.Id, cancellationToken);
                return userRole != null;
            }
            return false;
        }

        public override async Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(normalizedRoleName));
            }

            var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (roleEntity != null)
            {
                var userRole = await FindUserRoleAsync(user.Id, roleEntity.Id, cancellationToken);
                if (userRole != null)
                {
                    UserRoles.Remove(userRole);
                }
            }
        }

        protected override Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return Roles.SingleOrDefaultAsync(r =>
                r.NormalizedName == normalizedRoleName
                && (r.TenantId == TenantId || (r.TenantId == null)),
                cancellationToken);
        }

        protected override Task<IdentityUserRole<string>> FindUserRoleAsync(string userId, string roleId, CancellationToken cancellationToken)
        {
            return UserRoles.FindAsync(new object[] { userId, roleId }, cancellationToken);
        }
    }
}