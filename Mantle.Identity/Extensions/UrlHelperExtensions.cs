using Mantle.Identity;

namespace Microsoft.AspNetCore.Mvc;

public static class UrlHelperExtensions
{
    extension(IUrlHelper urlHelper)
    {
        public string EmailConfirmationLink<TUser>(string userId, string code, string scheme)
            where TUser : MantleIdentityUser, new() => urlHelper.Action(
                action: nameof(MantleAccountController<TUser>.ConfirmEmail),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);

        public string ResetPasswordCallbackLink<TUser>(string userId, string code, string scheme)
            where TUser : MantleIdentityUser, new() => urlHelper.Action(
                action: nameof(MantleAccountController<TUser>.ResetPassword),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
    }
}