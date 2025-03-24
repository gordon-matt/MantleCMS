using Mantle.Identity;

namespace Microsoft.AspNetCore.Mvc;

public static class UrlHelperExtensions
{
    public static string EmailConfirmationLink<TUser>(this IUrlHelper urlHelper, string userId, string code, string scheme)
        where TUser : MantleIdentityUser, new() => urlHelper.Action(
            action: nameof(MantleAccountController<TUser>.ConfirmEmail),
            controller: "Account",
            values: new { userId, code },
            protocol: scheme);

    public static string ResetPasswordCallbackLink<TUser>(this IUrlHelper urlHelper, string userId, string code, string scheme)
        where TUser : MantleIdentityUser, new() => urlHelper.Action(
            action: nameof(MantleAccountController<TUser>.ResetPassword),
            controller: "Account",
            values: new { userId, code },
            protocol: scheme);
}