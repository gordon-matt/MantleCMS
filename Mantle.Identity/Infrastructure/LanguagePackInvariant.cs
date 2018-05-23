using System.Collections.Generic;
using Mantle.Localization;

namespace Mantle.Identity.Infrastructure
{
    public class LanguagePackInvariant : ILanguagePack
    {
        #region ILanguagePack Members

        public string CultureCode => null;

        public IDictionary<string, string> LocalizedStrings => new Dictionary<string, string>
        {
            { LocalizableStrings.ConfirmNewPassword, "Confirm New Password" },
            { LocalizableStrings.ConfirmPassword, "Confirm Password" },
            { LocalizableStrings.EditMyProfile, "Edit My Profile" },
            { LocalizableStrings.EditProfile, "Edit Profile" },
            { LocalizableStrings.Email, "Email" },
            { LocalizableStrings.InvalidUserNameOrPassword, "Invalid username or password." },
            { LocalizableStrings.Login, "Log in" },
            { LocalizableStrings.LogOut, "Log out" },
            { LocalizableStrings.ManageMessages.ChangePasswordSuccess, "Your password has been changed." },
            { LocalizableStrings.ManageMessages.Error, "An error has occurred." },
            { LocalizableStrings.ManageMessages.RemoveLoginSuccess, "The external login was removed." },
            { LocalizableStrings.ManageMessages.SetPasswordSuccess, "Your password has been set." },
            { LocalizableStrings.MyProfile, "My Profile" },
            { LocalizableStrings.NewPassword, "New Password" },
            { LocalizableStrings.NoUserFound, "No user found." },
            { LocalizableStrings.OldPassword, "Current Password" },
            { LocalizableStrings.Password, "Password" },
            { LocalizableStrings.ProfileForUser, "Profile For '{0}'" },
            { LocalizableStrings.Register, "Register" },
            { LocalizableStrings.RememberMe, "Remember Me?" },
            { LocalizableStrings.Title, "Account" },
            { LocalizableStrings.UserDoesNotExistOrIsNotConfirmed, "The user either does not exist or is not confirmed." }
        };

        #endregion ILanguagePack Members
    }
}