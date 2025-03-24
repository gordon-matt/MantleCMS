using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Identity;

public abstract class MantleRoleStore<TRole, TContext> : RoleStore<TRole, TContext, string, IdentityUserRole<string>, IdentityRoleClaim<string>>
    where TRole : MantleIdentityRole
    where TContext : DbContext
{
    private IWorkContext workContext;

    public MantleRoleStore(TContext context, IdentityErrorDescriber describer = null)
        : base(context, describer)
    {
    }

    #region Private Properties

    private IWorkContext WorkContext
    {
        get
        {
            workContext ??= EngineContext.Current.Resolve<IWorkContext>();
            return workContext;
        }
    }

    private int TenantId => WorkContext.CurrentTenant.Id;

    #endregion Private Properties

    public override async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default) => await base.CreateAsync(role, cancellationToken);

    protected override IdentityRoleClaim<string> CreateRoleClaim(TRole role, Claim claim) => new() { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };

    // Get by ID should not need to override.. onyl for getting by name
    //public override Task<TRole> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
    //{
    //    cancellationToken.ThrowIfCancellationRequested();
    //    ThrowIfDisposed();
    //    var roleId = ConvertIdFromString(id);
    //    return Roles.FirstOrDefaultAsync(
    //        u =>
    //            u.Id.Equals(roleId)
    //            && (u.TenantId == TenantId || (u.TenantId == null)),
    //        cancellationToken);
    //}

    public override Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return Roles.FirstOrDefaultAsync(
            r =>
                r.NormalizedName == normalizedName
                && (r.TenantId == TenantId || (r.TenantId == null)),
            cancellationToken);
    }
}