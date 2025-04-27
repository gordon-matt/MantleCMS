using Mantle.Identity;
using Mantle.Identity.Models.AccountViewModels;
using Mantle.Identity.Services;
using Mantle.Security.Membership;
using Mantle.Web.Security.Membership;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MantleCMS.Controllers;

[Authorize]
[Route("account")]
public class AccountController : MantleAccountController<ApplicationUser>
{
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        ILogger<AccountController> logger,
        IMembershipService membershipService,
        Lazy<IEnumerable<IUserProfileProvider>> userProfileProviders)
        : base(userManager, signInManager, emailSender, logger, membershipService, userProfileProviders)
    {
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("login")]
    public override async Task<IActionResult> Login(string returnUrl = null) => await base.Login(returnUrl);

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [Route("login")]
    public override async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null) => await base.Login(model, returnUrl);

    [HttpGet]
    [AllowAnonymous]
    [Route("login-with-2fa")]
    public override async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null) => await base.LoginWith2fa(rememberMe, returnUrl);

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [Route("login-with-2fa")]
    public override async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null) => await base.LoginWith2fa(model, rememberMe, returnUrl);

    [HttpGet]
    [AllowAnonymous]
    [Route("login-with-recovery-code")]
    public override async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null) => await base.LoginWithRecoveryCode(returnUrl);

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [Route("login-with-recovery-code")]
    public override async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null) => await base.LoginWithRecoveryCode(model, returnUrl);

    [HttpGet]
    [AllowAnonymous]
    [Route("lockout")]
    public override IActionResult Lockout() => base.Lockout();

    [HttpGet]
    [AllowAnonymous]
    [Route("register")]
    public override IActionResult Register(string returnUrl = null) => base.Register(returnUrl);

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [Route("register")]
    public override async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null) => await base.Register(model, returnUrl);

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("logout")]
    public override async Task<IActionResult> Logout() => await base.Logout();

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [Route("external-login")]
    public override IActionResult ExternalLogin(string provider, string returnUrl = null) => base.ExternalLogin(provider, returnUrl);

    [HttpGet]
    [AllowAnonymous]
    [Route("external-login-callback")]
    public override async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null) => await base.ExternalLoginCallback(returnUrl, remoteError);

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [Route("external-login-confirmation")]
    public override async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null) => await base.ExternalLoginConfirmation(model, returnUrl);

    [HttpGet]
    [AllowAnonymous]
    [Route("confirm-email")]
    public override async Task<IActionResult> ConfirmEmail(string userId, string code) => await base.ConfirmEmail(userId, code);

    [HttpGet]
    [AllowAnonymous]
    [Route("forgot-password")]
    public override IActionResult ForgotPassword() => base.ForgotPassword();

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [Route("forgot-password")]
    public override async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model) => await base.ForgotPassword(model);

    [HttpGet]
    [AllowAnonymous]
    [Route("forgot-password-confirmation")]
    public override IActionResult ForgotPasswordConfirmation() => base.ForgotPasswordConfirmation();

    [HttpGet]
    [AllowAnonymous]
    [Route("reset-password")]
    public override IActionResult ResetPassword(string code = null) => base.ResetPassword(code);

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [Route("reset-password")]
    public override async Task<IActionResult> ResetPassword(ResetPasswordViewModel model) => await base.ResetPassword(model);

    [HttpGet]
    [AllowAnonymous]
    [Route("reset-password-confirmation")]
    public override IActionResult ResetPasswordConfirmation() => base.ResetPasswordConfirmation();

    [HttpGet]
    [Route("access-denied")]
    public override IActionResult AccessDenied() => base.AccessDenied();

    #region User Profile

    [Route("profile/{userId}")]
    public override async Task<IActionResult> ViewProfile(string userId) => await base.ViewProfile(userId);

    [Route("my-profile")]
    public override async Task<IActionResult> ViewMyProfile() => await base.ViewMyProfile();

    [Route("profile/edit/{userId}/")]
    public override async Task<IActionResult> EditProfile(string userId) => await base.EditProfile(userId);

    [Route("my-profile/edit")]
    public override async Task<IActionResult> EditMyProfile() => await base.EditMyProfile();

    [HttpPost]
    [Route("update-profile")]
    public override async Task<IActionResult> UpdateProfile() => await base.UpdateProfile();

    #endregion User Profile
}