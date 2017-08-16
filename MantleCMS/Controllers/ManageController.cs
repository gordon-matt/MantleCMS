using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Mantle.Identity;
using Mantle.Identity.Models.ManageViewModels;
using Mantle.Identity.Services;
using MantleCMS.Data.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MantleCMS.Controllers
{
    [Authorize]
    [Route("manage")]
    public class ManageController : MantleManageController<ApplicationUser>
    {
        public ManageController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<ManageController> logger,
            UrlEncoder urlEncoder)
            : base(userManager, signInManager, emailSender, logger, urlEncoder)
        {
        }

        [HttpGet]
        [Route("")]
        public override async Task<IActionResult> Index()
        {
            return await base.Index();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("")]
        public override async Task<IActionResult> Index(IndexViewModel model)
        {
            return await base.Index(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("send-verification-email")]
        public override async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            return await base.SendVerificationEmail(model);
        }

        [HttpGet]
        [Route("change-password")]
        public override async Task<IActionResult> ChangePassword()
        {
            return await base.ChangePassword();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("change-password")]
        public override async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            return await base.ChangePassword(model);
        }

        [HttpGet]
        [Route("set-password")]
        public override async Task<IActionResult> SetPassword()
        {
            return await base.SetPassword();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("set-password")]
        public override async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            return await base.SetPassword(model);
        }

        [HttpGet]
        [Route("external-logins")]
        public override async Task<IActionResult> ExternalLogins()
        {
            return await base.ExternalLogins();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("link-login")]
        public override async Task<IActionResult> LinkLogin(string provider)
        {
            return await base.LinkLogin(provider);
        }

        [HttpGet]
        [Route("link-login-callback")]
        public override async Task<IActionResult> LinkLoginCallback()
        {
            return await base.LinkLoginCallback();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("remove-login")]
        public override async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            return await base.RemoveLogin(model);
        }

        [HttpGet]
        [Route("two-factor-authentication")]
        public override async Task<IActionResult> TwoFactorAuthentication()
        {
            return await base.TwoFactorAuthentication();
        }

        [HttpGet]
        [Route("disable-2fa-warning")]
        public override async Task<IActionResult> Disable2faWarning()
        {
            return await base.Disable2faWarning();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("disable-2fa")]
        public override async Task<IActionResult> Disable2fa()
        {
            return await base.Disable2fa();
        }

        [HttpGet]
        [Route("enable-authenticator")]
        public override async Task<IActionResult> EnableAuthenticator()
        {
            return await base.EnableAuthenticator();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("enable-authenticator")]
        public override async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            return await base.EnableAuthenticator(model);
        }

        [HttpGet]
        [Route("reset-authenticator-warning")]
        public override IActionResult ResetAuthenticatorWarning()
        {
            return base.ResetAuthenticatorWarning();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("reset-authenticator")]
        public override async Task<IActionResult> ResetAuthenticator()
        {
            return await base.ResetAuthenticator();
        }

        [HttpGet]
        [Route("generate-recovery-codes")]
        public override async Task<IActionResult> GenerateRecoveryCodes()
        {
            return await base.GenerateRecoveryCodes();
        }
    }
}