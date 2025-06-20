﻿//TODO: Review all of this and replace some code with membershipService, same as in Mantle

namespace Mantle.Identity;

[Authorize]
public abstract class MantleAccountController<TUser> : MantleController
    where TUser : MantleIdentityUser, new()
{
    private readonly UserManager<TUser> _userManager;
    private readonly SignInManager<TUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger _logger;
    private readonly IMembershipService membershipService;
    private readonly Lazy<IEnumerable<IUserProfileProvider>> userProfileProviders;

    public MantleAccountController(
        UserManager<TUser> userManager,
        SignInManager<TUser> signInManager,
        IEmailSender emailSender,
        ILogger<MantleAccountController<TUser>> logger,
        IMembershipService membershipService,
        Lazy<IEnumerable<IUserProfileProvider>> userProfileProviders)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _logger = logger;
        this.membershipService = membershipService;
        this.userProfileProviders = userProfileProviders;
    }

    [TempData]
    public string ErrorMessage { get; set; }

    [HttpGet]
    [AllowAnonymous]
    public virtual async Task<IActionResult> Login(string returnUrl = null)
    {
        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }

        // If we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public virtual async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new ApplicationException($"Unable to load two-factor authentication user.");
        var model = new LoginWith2faViewModel { RememberMe = rememberMe };
        ViewData["ReturnUrl"] = returnUrl;

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        string authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

        if (result.Succeeded)
        {
            _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
            return RedirectToLocal(returnUrl);
        }
        else if (result.IsLockedOut)
        {
            _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
            return RedirectToAction(nameof(Lockout));
        }
        else
        {
            _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
            ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
            return View();
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public virtual async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync()
            ?? throw new ApplicationException($"Unable to load two-factor authentication user.");

        ViewData["ReturnUrl"] = returnUrl;

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync()
            ?? throw new ApplicationException($"Unable to load two-factor authentication user.");

        string recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

        var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        if (result.Succeeded)
        {
            _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
            return RedirectToLocal(returnUrl);
        }
        if (result.IsLockedOut)
        {
            _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
            return RedirectToAction(nameof(Lockout));
        }
        else
        {
            _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
            ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
            return View();
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public virtual IActionResult Lockout() => View();

    [HttpGet]
    [AllowAnonymous]
    public virtual IActionResult Register(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            var user = new TUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string callbackUrl = Url.EmailConfirmationLink<TUser>(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User created a new account with password.");
                return RedirectToLocal(returnUrl);
            }
            AddErrors(result);
        }

        // If we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public virtual IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        // Request a redirect to the external login provider.
        string redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    [AllowAnonymous]
    public virtual async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        if (remoteError != null)
        {
            ErrorMessage = $"Error from external provider: {remoteError}";
            return RedirectToAction(nameof(Login));
        }
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction(nameof(Login));
        }

        // Sign in the user with this external login provider if the user already has a login.
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
            return RedirectToLocal(returnUrl);
        }
        if (result.IsLockedOut)
        {
            return RedirectToAction(nameof(Lockout));
        }
        else
        {
            // If the user does not have an account, then ask the user to create an account.
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["LoginProvider"] = info.LoginProvider;
            string email = info.Principal.FindFirstValue(ClaimTypes.Email);
            return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
    {
        if (ModelState.IsValid)
        {
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync()
                ?? throw new ApplicationException("Error loading external login information during confirmation.");

            var user = new TUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                    return RedirectToLocal(returnUrl);
                }
            }
            AddErrors(result);
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(nameof(ExternalLogin), model);
    }

    [HttpGet]
    [AllowAnonymous]
    public virtual async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new ApplicationException($"Unable to load user with ID '{userId}'.");

        var result = await _userManager.ConfirmEmailAsync(user, code);
        return View(result.Succeeded ? "ConfirmEmail" : "Error");
    }

    [HttpGet]
    [AllowAnonymous]
    public virtual IActionResult ForgotPassword() => View();

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            string code = await _userManager.GeneratePasswordResetTokenAsync(user);
            string callbackUrl = Url.ResetPasswordCallbackLink<TUser>(user.Id, code, Request.Scheme);

            await _emailSender.SendEmailAsync(
                model.Email,
                "Reset Password",
                $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        // If we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public virtual IActionResult ForgotPasswordConfirmation() => View();

    [HttpGet]
    [AllowAnonymous]
    public virtual IActionResult ResetPassword(string code = null)
    {
        if (code == null)
        {
            throw new ApplicationException("A code must be supplied for password reset.");
        }
        var model = new ResetPasswordViewModel { Code = code };
        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }
        var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }
        AddErrors(result);
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public virtual IActionResult ResetPasswordConfirmation() => View();

    [HttpGet]
    public virtual IActionResult AccessDenied() => View();

    #region User Profile

    public virtual async Task<IActionResult> ViewProfile(string userId)
    {
        WorkContext.Breadcrumbs.Add(T[LocalizableStrings.Title].Value);

        if (userId == WorkContext.CurrentUser.Id)
        {
            ViewBag.Title = T[LocalizableStrings.MyProfile].Value;
            WorkContext.Breadcrumbs.Add(T[LocalizableStrings.MyProfile].Value);
            ViewBag.CanEdit = true;
        }
        else if (CheckPermission(StandardPermissions.FullAccess))
        {
            var user = await membershipService.GetUserById(userId);
            ViewBag.Title = string.Format(T[LocalizableStrings.ProfileForUser].Value, user.UserName);
            WorkContext.Breadcrumbs.Add(string.Format(T[LocalizableStrings.ProfileForUser].Value, user.UserName));
            ViewBag.CanEdit = true;
        }
        else
        {
            var user = await membershipService.GetUserById(userId);
            ViewBag.Title = string.Format(T[LocalizableStrings.ProfileForUser].Value, user.UserName);
            WorkContext.Breadcrumbs.Add(string.Format(T[LocalizableStrings.ProfileForUser].Value, user.UserName));
            ViewBag.CanEdit = false;
        }

        return View("Profile", model: userId);
    }

    public virtual async Task<IActionResult> ViewMyProfile() => await ViewProfile(WorkContext.CurrentUser.Id);

    public virtual async Task<IActionResult> EditProfile(string userId)
    {
        WorkContext.Breadcrumbs.Add(T[LocalizableStrings.Title].Value);

        if (userId == WorkContext.CurrentUser.Id)
        {
            ViewBag.Title = T[LocalizableStrings.EditMyProfile].Value;
            WorkContext.Breadcrumbs.Add(T[LocalizableStrings.MyProfile].Value, Url.Action("ViewMyProfile"));
            WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.General.Edit].Value);
        }
        else if (CheckPermission(StandardPermissions.FullAccess))
        {
            ViewBag.Title = T[LocalizableStrings.EditProfile].Value;
            var user = await membershipService.GetUserById(userId);
            WorkContext.Breadcrumbs.Add(string.Format(T[LocalizableStrings.ProfileForUser].Value, user.UserName), Url.Action("ViewProfile", new { userId }));
            WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.General.Edit].Value);
        }
        else
        {
            return Unauthorized();
        }

        return View("ProfileEdit", model: userId);
    }

    public virtual async Task<IActionResult> EditMyProfile() => await EditProfile(WorkContext.CurrentUser.Id);

    [HttpPost]
    public virtual async Task<IActionResult> UpdateProfile()
    {
        var userId = Request.Form["UserId"];

        var newProfile = new Dictionary<string, string>();

        foreach (var provider in userProfileProviders.Value)
        {
            foreach (string fieldName in provider.GetFieldNames())
            {
                string value = Request.Form[fieldName];

                if (value == "true,false")
                {
                    value = "true";
                }

                newProfile.Add(fieldName, value);
            }
        }

        await membershipService.UpdateProfile(userId, newProfile);

        //eventBus.Notify<IMembershipEventHandler>(x => x.ProfileChanged(userId, newProfile));

        return userId == WorkContext.CurrentUser.Id
            ? RedirectToAction("ViewMyProfile")
            : RedirectToAction("ViewMyProfile", RouteData.Values.Merge(new { userId }));
    }

    #endregion User Profile

    #region Helpers

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    private IActionResult RedirectToLocal(string returnUrl) => Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");

    #endregion Helpers
}