using Microsoft.EntityFrameworkCore;

namespace Mantle.Identity;

public abstract class MantleRoleValidator<TRole> : RoleValidator<TRole>
    where TRole : MantleIdentityRole
{
    public MantleRoleValidator(IdentityErrorDescriber errors = null)
        : base(errors)
    {
        Describer = errors ?? new IdentityErrorDescriber();
    }

    private IdentityErrorDescriber Describer { get; set; }

    public override async Task<IdentityResult> ValidateAsync(RoleManager<TRole> manager, TRole role)
    {
        if (manager == null)
        {
            throw new ArgumentNullException(nameof(manager));
        }
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }
        var errors = new List<IdentityError>();
        await ValidateRoleName(manager, role, errors);
        if (errors.Count > 0)
        {
            return IdentityResult.Failed(errors.ToArray());
        }
        return IdentityResult.Success;
    }

    private async Task ValidateRoleName(
        RoleManager<TRole> manager,
        TRole role,
        ICollection<IdentityError> errors)
    {
        string roleName = await manager.GetRoleNameAsync(role);
        if (string.IsNullOrWhiteSpace(roleName))
        {
            errors.Add(Describer.InvalidRoleName(roleName));
        }
        else
        {
            var owner = await manager.Roles.FirstOrDefaultAsync(x => x.TenantId == role.TenantId && x.NormalizedName == roleName);

            if (owner != null &&
                !string.Equals(await manager.GetRoleIdAsync(owner), await manager.GetRoleIdAsync(role)))
            {
                errors.Add(Describer.DuplicateRoleName(roleName));
            }
        }
    }
}