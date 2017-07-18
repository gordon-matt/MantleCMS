using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mantle.Identity;
using Mantle.Identity.Models.AccountViewModels;
using Mantle.Identity.Services;
using Mantle.Security.Membership;
using Mantle.Web.Security.Membership;
using MantleCMS.Data.Domain;
using MantleCMS.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MantleCMS.Controllers
{
    [Authorize]
    [Route("account")]
    public class AccountController : MantleAccountController<ApplicationUser>
    {
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            IMembershipService membershipService,
            Lazy<IEnumerable<IUserProfileProvider>> userProfileProviders)
            : base(userManager, signInManager, emailSender, smsSender, loggerFactory, membershipService, userProfileProviders)
        {
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        [Route("login")]
        public override IActionResult Login(string returnUrl = null)
        {
            StartupTask.Execute(); // <-- Temporary until installation page is done...
            return base.Login(returnUrl);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("login")]
        public override async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            return await base.Login(model, returnUrl);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        [Route("register")]
        public override IActionResult Register(string returnUrl = null)
        {
            return base.Register(returnUrl);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("register")]
        public override async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            return await base.Register(model, returnUrl);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("log-off")]
        public override async Task<IActionResult> LogOff()
        {
            return await base.LogOff();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("external-login")]
        public override IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            return base.ExternalLogin(provider, returnUrl);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        [Route("external-login-callback")]
        public override async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            return await base.ExternalLoginCallback(returnUrl, remoteError);
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("external-login-confirmation")]
        public override async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            return await base.ExternalLoginConfirmation(model, returnUrl);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        [Route("confirm-email")]
        public override async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            return await base.ConfirmEmail(userId, code);
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        [Route("forgot-password")]
        public override IActionResult ForgotPassword()
        {
            return base.ForgotPassword();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("forgot-password")]
        public override async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            return await base.ForgotPassword(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        [Route("forgot-password-confirmation")]
        public override IActionResult ForgotPasswordConfirmation()
        {
            return base.ForgotPasswordConfirmation();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        [Route("reset-password")]
        public override IActionResult ResetPassword(string code = null)
        {
            return base.ResetPassword(code);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("reset-password")]
        public override async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            return await base.ResetPassword(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        [Route("reset-password-confirmation")]
        public override IActionResult ResetPasswordConfirmation()
        {
            return base.ResetPasswordConfirmation();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        [Route("send-code")]
        public override async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            return await base.SendCode(returnUrl, rememberMe);
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("send-code")]
        public override async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            return await base.SendCode(model);
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        [Route("verify-code")]
        public override async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            return await base.VerifyCode(provider, rememberMe, returnUrl);
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("verify-code")]
        public override async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            return await base.VerifyCode(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("access-denied")]
        public virtual IActionResult AccessDenied(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        #region User Profile

        [Route("profile/{userId}")]
        public override async Task<ActionResult> ViewProfile(string userId)
        {
            return await base.ViewProfile(userId);
        }

        [Route("my-profile")]
        public override async Task<ActionResult> ViewMyProfile()
        {
            return await base.ViewMyProfile();
        }

        [Route("profile/edit/{userId}/")]
        public override async Task<ActionResult> EditProfile(string userId)
        {
            return await base.EditProfile(userId);
        }

        [Route("my-profile/edit")]
        public override async Task<ActionResult> EditMyProfile()
        {
            return await base.EditMyProfile();
        }

        [HttpPost]
        [Route("update-profile")]
        public override async Task<ActionResult> UpdateProfile()
        {
            return await base.UpdateProfile();
        }

        #endregion User Profile
    }
}