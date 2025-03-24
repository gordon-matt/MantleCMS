using System.Text.Encodings.Web;
using Mantle.Identity;
using Mantle.Identity.Models.ManageViewModels;
using Mantle.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MantleCMS.Controllers;

[Authorize]
[Route("manage")]
public class ManageController : MantleManageController<ApplicationUser>
{
    public ManageController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        ILogger<ManageController> logger,
        UrlEncoder urlEncoder,
        SiteSettings siteSettings)
        : base(userManager, signInManager, emailSender, logger, urlEncoder, siteSettings)
    {
    }

    [HttpGet]
    [Route("")]
    public override async Task<IActionResult> Index() => await base.Index();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("")]
    public override async Task<IActionResult> Index(IndexViewModel model) => await base.Index(model);

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("send-verification-email")]
    public override async Task<IActionResult> SendVerificationEmail(IndexViewModel model) => await base.SendVerificationEmail(model);

    [HttpGet]
    [Route("change-password")]
    public override async Task<IActionResult> ChangePassword() => await base.ChangePassword();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("change-password")]
    public override async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) => await base.ChangePassword(model);

    [HttpGet]
    [Route("set-password")]
    public override async Task<IActionResult> SetPassword() => await base.SetPassword();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("set-password")]
    public override async Task<IActionResult> SetPassword(SetPasswordViewModel model) => await base.SetPassword(model);

    [HttpGet]
    [Route("external-logins")]
    public override async Task<IActionResult> ExternalLogins() => await base.ExternalLogins();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("link-login")]
    public override async Task<IActionResult> LinkLogin(string provider) => await base.LinkLogin(provider);

    [HttpGet]
    [Route("link-login-callback")]
    public override async Task<IActionResult> LinkLoginCallback() => await base.LinkLoginCallback();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("remove-login")]
    public override async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model) => await base.RemoveLogin(model);

    [HttpGet]
    [Route("two-factor-authentication")]
    public override async Task<IActionResult> TwoFactorAuthentication() => await base.TwoFactorAuthentication();

    [HttpGet]
    [Route("disable-2fa-warning")]
    public override async Task<IActionResult> Disable2faWarning() => await base.Disable2faWarning();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("disable-2fa")]
    public override async Task<IActionResult> Disable2fa() => await base.Disable2fa();

    [HttpGet]
    [Route("enable-authenticator")]
    public override async Task<IActionResult> EnableAuthenticator() => await base.EnableAuthenticator();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("enable-authenticator")]
    public override async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model) => await base.EnableAuthenticator(model);

    [HttpGet]
    [Route("reset-authenticator-warning")]
    public override IActionResult ResetAuthenticatorWarning() => base.ResetAuthenticatorWarning();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("reset-authenticator")]
    public override async Task<IActionResult> ResetAuthenticator() => await base.ResetAuthenticator();

    [HttpGet]
    [Route("generate-recovery-codes")]
    public override async Task<IActionResult> GenerateRecoveryCodes() => await base.GenerateRecoveryCodes();
}