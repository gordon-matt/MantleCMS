//using Mantle.DI;
using Mantle.Security.Membership;

namespace Mantle.Web.Security.Membership.Permissions
{
    /// <summary>
    /// Entry-point for configured authorization scheme. Role-based system provided by default.
    /// </summary>
    public interface IAuthorizationService //: IDependency
    {
        void CheckAccess(Permission permission, MantleUser user);

        bool TryCheckAccess(Permission permission, MantleUser user);
    }
}