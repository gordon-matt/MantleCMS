using System.Text;
using System.Text.Encodings.Web;
using Mantle.Identity.Models.ManageViewModels;
using Mantle.Web.Configuration;

namespace Mantle.Identity;

[Authorize]
public abstract class MantleManageController<TUser> : Controller
    where TUser : MantleIdentityUser, new()
{
    private readonly UserManager<TUser> userManager;
    private readonly SignInManager<TUser> signInManager;
    private readonly IEmailSender emailSender;
    private readonly ILogger logger;
    private readonly UrlEncoder urlEncoder;
    private readonly SiteSettings siteSettings;

    private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    public MantleManageController(
      UserManager<TUser> userManager,
      SignInManager<TUser> signInManager,
      IEmailSender emailSender,
      ILogger<MantleManageController<TUser>> logger,
      UrlEncoder urlEncoder,
      SiteSettings siteSettings)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.emailSender = emailSender;
        this.logger = logger;
        this.urlEncoder = urlEncoder;
        this.siteSettings = siteSettings;
    }

    [TempData]
    public string StatusMessage { get; set; }

    [HttpGet]
    public virtual async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        var model = new IndexViewModel
        {
            Username = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IsEmailConfirmed = user.EmailConfirmed,
            StatusMessage = StatusMessage
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Index(IndexViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        string email = user.Email;
        if (model.Email != email)
        {
            var setEmailResult = await userManager.SetEmailAsync(user, model.Email);
            if (!setEmailResult.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
            }
        }

        string phoneNumber = user.PhoneNumber;
        if (model.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
            }
        }

        StatusMessage = "Your profile has been updated";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        string callbackUrl = Url.EmailConfirmationLink<TUser>(user.Id, code, Request.Scheme);
        string email = user.Email;
        await emailSender.SendEmailConfirmationAsync(email, callbackUrl);

        StatusMessage = "Verification email sent. Please check your email.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public virtual async Task<IActionResult> ChangePassword()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        bool hasPassword = await userManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            return RedirectToAction(nameof(SetPassword));
        }

        var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            AddErrors(changePasswordResult);
            return View(model);
        }

        await signInManager.SignInAsync(user, isPersistent: false);
        logger.LogInformation("User changed their password successfully.");
        StatusMessage = "Your password has been changed.";

        return RedirectToAction(nameof(ChangePassword));
    }

    [HttpGet]
    public virtual async Task<IActionResult> SetPassword()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        bool hasPassword = await userManager.HasPasswordAsync(user);

        if (hasPassword)
        {
            return RedirectToAction(nameof(ChangePassword));
        }

        var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> SetPassword(SetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.GetUserAsync(User)
            ?? throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        var addPasswordResult = await userManager.AddPasswordAsync(user, model.NewPassword);
        if (!addPasswordResult.Succeeded)
        {
            AddErrors(addPasswordResult);
            return View(model);
        }

        await signInManager.SignInAsync(user, isPersistent: false);
        StatusMessage = "Your password has been set.";

        return RedirectToAction(nameof(SetPassword));
    }

    [HttpGet]
    public virtual async Task<IActionResult> ExternalLogins()
    {
        var user = await userManager.GetUserAsync(User)
            ?? throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        var model = new ExternalLoginsViewModel { CurrentLogins = await userManager.GetLoginsAsync(user) };
        model.OtherLogins = (await signInManager.GetExternalAuthenticationSchemesAsync())
            .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
            .ToList();
        model.ShowRemoveButton = await userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
        model.StatusMessage = StatusMessage;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> LinkLogin(string provider)
    {
        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        // Request a redirect to the external login provider to link a login for the current user
        string redirectUrl = Url.Action(nameof(LinkLoginCallback));
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userManager.GetUserId(User));
        return new ChallengeResult(provider, properties);
    }

    [HttpGet]
    public virtual async Task<IActionResult> LinkLoginCallback()
    {
        var user = await userManager.GetUserAsync(User)
            ?? throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        var info = await signInManager.GetExternalLoginInfoAsync(user.Id);
        if (info == null)
        {
            throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
        }

        var result = await userManager.AddLoginAsync(user, info);
        if (!result.Succeeded)
        {
            throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
        }

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        StatusMessage = "The external login was added.";
        return RedirectToAction(nameof(ExternalLogins));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
    {
        var user = await userManager.GetUserAsync(User)
            ?? throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        var result = await userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
        if (!result.Succeeded)
        {
            throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
        }

        await signInManager.SignInAsync(user, isPersistent: false);
        StatusMessage = "The external login was removed.";
        return RedirectToAction(nameof(ExternalLogins));
    }

    [HttpGet]
    public virtual async Task<IActionResult> TwoFactorAuthentication()
    {
        var user = await userManager.GetUserAsync(User)
            ?? throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        var model = new TwoFactorAuthenticationViewModel
        {
            HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) != null,
            Is2faEnabled = user.TwoFactorEnabled,
            RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user),
        };

        return View(model);
    }

    [HttpGet]
    public virtual async Task<IActionResult> Disable2faWarning()
    {
        var user = await userManager.GetUserAsync(User);
        return user == null
            ? throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.")
            : !user.TwoFactorEnabled
                ? throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.")
                : (IActionResult)View(nameof(Disable2fa));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Disable2fa()
    {
        var user = await userManager.GetUserAsync(User)
            ?? throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        var disable2faResult = await userManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2faResult.Succeeded)
        {
            throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
        }

        logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
        return RedirectToAction(nameof(TwoFactorAuthentication));
    }

    [HttpGet]
    public virtual async Task<IActionResult> EnableAuthenticator()
    {
        var user = await userManager.GetUserAsync(User)
            ?? throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        string unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        }

        var model = new EnableAuthenticatorViewModel
        {
            SharedKey = FormatKey(unformattedKey),
            AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userManager.GetUserAsync(User)
            ?? throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        // Strip spaces and hypens
        string verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        bool is2faTokenValid = await userManager.VerifyTwoFactorTokenAsync(
            user, userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2faTokenValid)
        {
            ModelState.AddModelError("model.Code", "Verification code is invalid.");
            return View(model);
        }

        await userManager.SetTwoFactorEnabledAsync(user, true);
        logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
        return RedirectToAction(nameof(GenerateRecoveryCodes));
    }

    [HttpGet]
    public virtual IActionResult ResetAuthenticatorWarning() => View(nameof(ResetAuthenticator));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> ResetAuthenticator()
    {
        var user = await userManager.GetUserAsync(User)
            ?? throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        await userManager.SetTwoFactorEnabledAsync(user, false);
        await userManager.ResetAuthenticatorKeyAsync(user);
        logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

        return RedirectToAction(nameof(EnableAuthenticator));
    }

    [HttpGet]
    public virtual async Task<IActionResult> GenerateRecoveryCodes()
    {
        var user = await userManager.GetUserAsync(User)
            ?? throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        if (!user.TwoFactorEnabled)
        {
            throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");
        }

        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        var model = new GenerateRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

        logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

        return View(model);
    }

    #region Helpers

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    private string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.Substring(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey[currentPosition..]);
        }

        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string email, string unformattedKey) => string.Format(
        AuthenicatorUriFormat,
        urlEncoder.Encode(siteSettings.SiteName),
        urlEncoder.Encode(email),
        unformattedKey);

    #endregion Helpers
}