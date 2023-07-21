using Extenso.Collections;
using Mantle.Threading;
using System.Security;

namespace Mantle.Security.Membership.Permissions
{
    public class RolesBasedAuthorizationService : IAuthorizationService
    {
        private readonly IMembershipService membershipService;

        private static readonly string[] anonymousRole = new[] { "Anonymous" };
        private static readonly string[] authenticatedRole = new[] { "Authenticated" };

        public RolesBasedAuthorizationService(IMembershipService membershipService)
        {
            this.membershipService = membershipService;
        }

        public void CheckAccess(Permission permission, MantleUser user)
        {
            if (!TryCheckAccess(permission, user))
            {
                throw new SecurityException();
            }
        }

        public bool TryCheckAccess(Permission permission, MantleUser user)
        {
            if (user == null || permission == null)
            {
                return false;
            }

            var context = new CheckAccessContext { Permission = permission, User = user };

            for (var adjustmentLimiter = 0; adjustmentLimiter != 3; ++adjustmentLimiter)
            {
                if (!context.Granted)
                {
                    // determine which set of permissions would satisfy the access check
                    var grantingNames = PermissionNames(context.Permission, Enumerable.Empty<string>()).Distinct().ToArray();

                    // determine what set of roles should be examined by the access check
                    IEnumerable<string> rolesToExamine;
                    if (context.User == null)
                    {
                        rolesToExamine = anonymousRole;
                    }
                    else
                    {
                        rolesToExamine = (AsyncHelper.RunSync(() => membershipService.GetRolesForUser(context.User.Id))).Select(x => x.Name).ToList();
                        if (!rolesToExamine.Contains(anonymousRole[0]))
                        {
                            rolesToExamine = rolesToExamine.Concat(authenticatedRole);
                        }
                    }

                    foreach (var role in rolesToExamine)
                    {
                        //var rolePermissions = AsyncHelper.RunSync(() => membershipService.GetPermissionsForRole(workContext.CurrentTenant.Id, role));
                        var rolePermissions = AsyncHelper.RunSync(() => membershipService.GetPermissionsForRole(user.TenantId, role));
                        foreach (var rolePermission in rolePermissions)
                        {
                            string possessedName = rolePermission.Name;
                            if (grantingNames.Any(grantingName => string.Equals(possessedName, grantingName, StringComparison.OrdinalIgnoreCase)))
                            {
                                context.Granted = true;
                            }

                            if (context.Granted)
                            {
                                break;
                            }
                        }

                        if (context.Granted)
                        {
                            break;
                        }
                    }
                }

                context.Adjusted = false;

                if (!context.Adjusted)
                {
                    break;
                }
            }

            return context.Granted;
        }

        private static IEnumerable<string> PermissionNames(Permission permission, IEnumerable<string> stack)
        {
            // the given name is tested
            yield return permission.Name;

            // iterate implied permissions to grant, it present
            if (!permission.ImpliedBy.IsNullOrEmpty())
            {
                foreach (var impliedBy in permission.ImpliedBy)
                {
                    // avoid potential recursion
                    if (stack.Contains(impliedBy.Name))
                    {
                        continue;
                    }

                    // otherwise accumulate the implied permission names recursively
                    foreach (var impliedName in PermissionNames(impliedBy, stack.Concat(new[] { permission.Name })))
                    {
                        yield return impliedName;
                    }
                }
            }

            yield return StandardPermissions.FullAccess.Name;
        }
    }
}