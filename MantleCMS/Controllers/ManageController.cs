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
            ISmsSender smsSender,
            ILoggerFactory loggerFactory)
            : base(userManager, signInManager, emailSender, smsSender, loggerFactory)
        {
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        [Route("")]
        public override async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            return await base.Index(message);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("remove-login")]
        public override async Task<IActionResult> RemoveLogin(RemoveLoginViewModel account)
        {
            return await base.RemoveLogin(account);
        }

        //
        // GET: /Manage/AddPhoneNumber
        [Route("add-phone-number")]
        public override IActionResult AddPhoneNumber()
        {
            return base.AddPhoneNumber();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add-phone-number")]
        public override async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            return await base.AddPhoneNumber(model);
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("enable-two-factor-authentication")]
        public override async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            return await base.EnableTwoFactorAuthentication();
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("disable-two-factor-authentication")]
        public override async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            return await base.DisableTwoFactorAuthentication();
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        [HttpGet]
        [Route("verify-phone-number")]
        public override async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            return await base.VerifyPhoneNumber(phoneNumber);
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("verify-phone-number")]
        public override async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            return await base.VerifyPhoneNumber(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("remove-phone-number")]
        public override async Task<IActionResult> RemovePhoneNumber()
        {
            return await base.RemovePhoneNumber();
        }

        //
        // GET: /Manage/ChangePassword
        [HttpGet]
        [Route("change-password")]
        public override IActionResult ChangePassword()
        {
            return base.ChangePassword();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("change-password")]
        public override async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            return await base.ChangePassword(model);
        }

        //
        // GET: /Manage/SetPassword
        [HttpGet]
        [Route("set-password")]
        public override IActionResult SetPassword()
        {
            return base.SetPassword();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("set-password")]
        public override async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            return await base.SetPassword(model);
        }

        //GET: /Manage/ManageLogins
        [HttpGet]
        [Route("manage-logins")]
        public override async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
        {
            return await base.ManageLogins(message);
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("link-login")]
        public override IActionResult LinkLogin(string provider)
        {
            return base.LinkLogin(provider);
        }

        //
        // GET: /Manage/LinkLoginCallback
        [HttpGet]
        [Route("link-login-callback")]
        public override async Task<ActionResult> LinkLoginCallback()
        {
            return await base.LinkLoginCallback();
        }
    }
}