﻿using Mantle.Localization;

namespace Mantle.Web.Infrastructure
{
    public class LanguagePackInvariant : ILanguagePack
    {
        #region ILanguagePack Members

        public string CultureCode => null;

        public IDictionary<string, string> LocalizedStrings => new Dictionary<string, string>
        {
            { MantleWebLocalizableStrings.Dashboard.Administration, "Administration" },
            { MantleWebLocalizableStrings.Dashboard.Frontend, "Frontend" },
            { MantleWebLocalizableStrings.Dashboard.Title, "Dashboard" },

            { MantleWebLocalizableStrings.General.Actions, "Actions" },
            { MantleWebLocalizableStrings.General.Add, "Add" },
            { MantleWebLocalizableStrings.General.AreYouSure, "Are you sure?" },
            { MantleWebLocalizableStrings.General.Back, "Back" },
            { MantleWebLocalizableStrings.General.Cancel, "Cancel" },
            { MantleWebLocalizableStrings.General.Clear, "Clear" },
            { MantleWebLocalizableStrings.General.Close, "Close" },
            { MantleWebLocalizableStrings.General.Configuration, "Configuration" },
            { MantleWebLocalizableStrings.General.Confirm, "Confirm" },
            { MantleWebLocalizableStrings.General.ConfirmDeleteRecord, "Are you sure that you want to delete this record?" },
            { MantleWebLocalizableStrings.General.Continue, "Continue" },
            { MantleWebLocalizableStrings.General.Create, "Create" },
            { MantleWebLocalizableStrings.General.CreateFormat, "Create {0}" },
            { MantleWebLocalizableStrings.General.DateCreated, "Date Created" },
            { MantleWebLocalizableStrings.General.DateCreatedUtc, "Date Created (UTC)" },
            { MantleWebLocalizableStrings.General.DateModified, "Date Modified" },
            { MantleWebLocalizableStrings.General.DateModifiedUtc, "Date Modified (UTC)" },
            { MantleWebLocalizableStrings.General.Delete, "Delete" },
            { MantleWebLocalizableStrings.General.DeleteRecordError, "There was an error when deleting the record." },
            { MantleWebLocalizableStrings.General.DeleteRecordErrorFormat, "There was an error when deleting the record. Additional information as follows: {0}" },
            { MantleWebLocalizableStrings.General.DeleteRecordSuccess, "Successfully deleted record." },
            { MantleWebLocalizableStrings.General.Description, "Description" },
            { MantleWebLocalizableStrings.General.Details, "Details" },
            { MantleWebLocalizableStrings.General.Download, "Download" },
            { MantleWebLocalizableStrings.General.Edit, "Edit" },
            { MantleWebLocalizableStrings.General.EditFormat, "Edit {0}" },
            { MantleWebLocalizableStrings.General.EditWithFormat, "Edit with {0}" },
            { MantleWebLocalizableStrings.General.Enabled, "Enabled" },
            { MantleWebLocalizableStrings.General.Error, "Error" },
            { MantleWebLocalizableStrings.General.Export, "Export" },
            { MantleWebLocalizableStrings.General.GetRecordError, "There was an error when retrieving the record." },
            { MantleWebLocalizableStrings.General.Home, "Home" },
            { MantleWebLocalizableStrings.General.Id, "ID" },
            { MantleWebLocalizableStrings.General.Import, "Import" },
            { MantleWebLocalizableStrings.General.InsertRecordError, "There was an error when inserting the record." },
            { MantleWebLocalizableStrings.General.InsertRecordSuccess, "Successfully inserted record." },
            { MantleWebLocalizableStrings.General.Install, "Install" },
            { MantleWebLocalizableStrings.General.Loading, "Loading…" },
            { MantleWebLocalizableStrings.General.Localize, "Localize" },
            { MantleWebLocalizableStrings.General.Miscellaneous, "Miscellaneous" },
            { MantleWebLocalizableStrings.General.Move, "Move" },
            { MantleWebLocalizableStrings.General.Name, "Name" },
            { MantleWebLocalizableStrings.General.Next, "Next" },
            { MantleWebLocalizableStrings.General.None, "None" },
            { MantleWebLocalizableStrings.General.OK, "OK" },
            { MantleWebLocalizableStrings.General.OnOff, "On/Off" },
            { MantleWebLocalizableStrings.General.Order, "Order" },
            { MantleWebLocalizableStrings.General.PleaseWait, "Please Wait…" },
            { MantleWebLocalizableStrings.General.Preview, "Preview" },
            { MantleWebLocalizableStrings.General.Previous, "Previous" },
            { MantleWebLocalizableStrings.General.Remove, "Remove" },
            { MantleWebLocalizableStrings.General.Rename, "Rename" },
            { MantleWebLocalizableStrings.General.Save, "Save" },
            { MantleWebLocalizableStrings.General.SaveAndContinue, "Save & Continue" },
            { MantleWebLocalizableStrings.General.Search, "Search" },
            { MantleWebLocalizableStrings.General.SelectOption, "Select…" },
            { MantleWebLocalizableStrings.General.Set, "Set" },
            { MantleWebLocalizableStrings.General.SetDefault, "Set Default" },
            { MantleWebLocalizableStrings.General.Settings, "Settings" },
            { MantleWebLocalizableStrings.General.Slug, "URL Slug" },
            { MantleWebLocalizableStrings.General.Submit, "Submit" },
            { MantleWebLocalizableStrings.General.Success, "Success" },
            { MantleWebLocalizableStrings.General.Themes, "Themes" },
            { MantleWebLocalizableStrings.General.Toggle, "On/Off" },
            { MantleWebLocalizableStrings.General.Uninstall, "Uninstall" },
            { MantleWebLocalizableStrings.General.Unspecified, "Unspecified" },
            { MantleWebLocalizableStrings.General.UpdateRecordError, "There was an error when updating the record." },
            { MantleWebLocalizableStrings.General.UpdateRecordErrorFormat, "There was an error when updating the record. Additional information as follows: {0}" },
            { MantleWebLocalizableStrings.General.UpdateRecordSuccess, "Successfully updated record." },
            { MantleWebLocalizableStrings.General.Upload, "Upload" },
            { MantleWebLocalizableStrings.General.View, "View" },
            { MantleWebLocalizableStrings.General.ViewFormat, "View {0}" },

            { MantleWebLocalizableStrings.Localization.IsRTL, "Is Right-to-Left" },
            { MantleWebLocalizableStrings.Localization.LanguageModel.CultureCode, "Culture" },
            { MantleWebLocalizableStrings.Localization.LanguageModel.IsEnabled, "Enabled" },
            { MantleWebLocalizableStrings.Localization.LanguageModel.IsRTL, "Right-to-Left" },
            { MantleWebLocalizableStrings.Localization.LanguageModel.Name, "Name" },
            { MantleWebLocalizableStrings.Localization.LanguageModel.SortOrder, "Sort Order" },
            { MantleWebLocalizableStrings.Localization.Languages, "Languages" },
            { MantleWebLocalizableStrings.Localization.LocalizableStringModel.InvariantValue, "Invariant" },
            { MantleWebLocalizableStrings.Localization.LocalizableStringModel.Key, "Key" },
            { MantleWebLocalizableStrings.Localization.LocalizableStringModel.LocalizedValue, "Localized" },
            { MantleWebLocalizableStrings.Localization.LocalizableStrings, "Localizable Strings" },
            { MantleWebLocalizableStrings.Localization.Localize, "Localize" },
            { MantleWebLocalizableStrings.Localization.ManageLanguages, "Manage Languages" },
            { MantleWebLocalizableStrings.Localization.ManageLocalizableStrings, "Manage Localizable Strings" },
            { MantleWebLocalizableStrings.Localization.SelectLanguage, "Select Language" },
            { MantleWebLocalizableStrings.Localization.Title, "Localization" },
            { MantleWebLocalizableStrings.Localization.Translate, "Translate" },
            { MantleWebLocalizableStrings.Localization.Translations, "Translations" },

            { MantleWebLocalizableStrings.Localization.ResetLocalizableStringsConfirm, "This will clear all the localizable strings in the database. Are you sure you want to do this?" },
            { MantleWebLocalizableStrings.Localization.ResetLocalizableStringsError, "There was an error when resetting the localized strings." },
            { MantleWebLocalizableStrings.Localization.ResetLocalizableStringsSuccess, "Successfully reset the localized strings." },

            { MantleWebLocalizableStrings.Log.ClearConfirm, "Are you sure you want to clear the log?" },
            { MantleWebLocalizableStrings.Log.ClearError, "Error when trying to clear the log." },
            { MantleWebLocalizableStrings.Log.ClearSuccess, "Successfully cleared the log." },
            { MantleWebLocalizableStrings.Log.Title, "Log" },
            { MantleWebLocalizableStrings.Log.Model.ErrorClass, "Error Class" },
            { MantleWebLocalizableStrings.Log.Model.ErrorMessage, "Error Message" },
            { MantleWebLocalizableStrings.Log.Model.ErrorMethod, "Error Method" },
            { MantleWebLocalizableStrings.Log.Model.ErrorSource, "Error Source" },
            { MantleWebLocalizableStrings.Log.Model.EventDateTime, "Event Date/Time" },
            { MantleWebLocalizableStrings.Log.Model.EventLevel, "Event Level" },
            { MantleWebLocalizableStrings.Log.Model.EventMessage, "Event Message" },
            { MantleWebLocalizableStrings.Log.Model.InnerErrorMessage, "Inner Error Message" },
            { MantleWebLocalizableStrings.Log.Model.MachineName, "Machine Name" },
            { MantleWebLocalizableStrings.Log.Model.UserName, "Username" },

            { MantleWebLocalizableStrings.Maintenance.Title, "Maintenance" },

            { MantleWebLocalizableStrings.Membership.AllRolesSelectListOption, "[All Roles]" },
            { MantleWebLocalizableStrings.Membership.ChangePassword, "Change Password" },
            { MantleWebLocalizableStrings.Membership.ChangePasswordError, "There was an error when changing the password." },
            { MantleWebLocalizableStrings.Membership.ChangePasswordModel.ConfirmPassword, "Confirm Password" },
            { MantleWebLocalizableStrings.Membership.ChangePasswordModel.Password, "Password" },
            { MantleWebLocalizableStrings.Membership.ChangePasswordModel.UserName, "Username" },
            { MantleWebLocalizableStrings.Membership.ChangePasswordSuccess, "Successfully changed password." },
            { MantleWebLocalizableStrings.Membership.EditRolePermissions, "Edit Role Permissions" },
            { MantleWebLocalizableStrings.Membership.InsertUserError, "There was an error when trying to create the user." },
            { MantleWebLocalizableStrings.Membership.InvalidEmailAddress, "That is not a valid e-mail address. Please check your input and try again." },
            { MantleWebLocalizableStrings.Membership.IsLockedOut, "Is Locked Out" },
            { MantleWebLocalizableStrings.Membership.Password, "Password" },
            { MantleWebLocalizableStrings.Membership.PermissionModel.Category, "Category" },
            { MantleWebLocalizableStrings.Membership.PermissionModel.Name, "Name" },
            { MantleWebLocalizableStrings.Membership.Permissions, "Permissions" },
            { MantleWebLocalizableStrings.Membership.RoleModel.Name, "Name" },
            { MantleWebLocalizableStrings.Membership.Roles, "Roles" },
            { MantleWebLocalizableStrings.Membership.SavePermissionsError, "There was an error when saving permissions." },
            { MantleWebLocalizableStrings.Membership.SavePermissionsSuccess, "Successfully saved permissions." },
            { MantleWebLocalizableStrings.Membership.SaveRolesError, "There was an error when saving roles." },
            { MantleWebLocalizableStrings.Membership.SaveRolesSuccess, "Successfully saved roles." },
            { MantleWebLocalizableStrings.Membership.Title, "Membership" },
            { MantleWebLocalizableStrings.Membership.UpdateUserRoles, "Update User Roles" },
            { MantleWebLocalizableStrings.Membership.UserEmailAlreadyExists, "A user with that e-mail address already exists. If you are the owner of that e-mail address, please login and try again." },
            { MantleWebLocalizableStrings.Membership.UserModel.Email, "Email" },
            { MantleWebLocalizableStrings.Membership.UserModel.IsActive, "Is Active" },
            { MantleWebLocalizableStrings.Membership.UserModel.IsLockedOut, "Is Locked Out" },
            { MantleWebLocalizableStrings.Membership.UserModel.Roles, "Roles" },
            { MantleWebLocalizableStrings.Membership.UserModel.UserName, "Username" },
            { MantleWebLocalizableStrings.Membership.Users, "Users" },

            { MantleWebLocalizableStrings.Plugins.InstallPluginError, "There was an error when installing the plugin." },
            { MantleWebLocalizableStrings.Plugins.InstallPluginSuccess, "Successfully installed the specified plugin." },
            { MantleWebLocalizableStrings.Plugins.ManagePlugins, "Manage Plugins" },
            { MantleWebLocalizableStrings.Plugins.Title, "Plugins" },
            { MantleWebLocalizableStrings.Plugins.Model.Group, "Group" },
            { MantleWebLocalizableStrings.Plugins.Model.PluginInfo, "Plugin Info" },
            { MantleWebLocalizableStrings.Plugins.Model.DisplayOrder, "Display Order" },
            { MantleWebLocalizableStrings.Plugins.Model.FriendlyName, "Friendly Name" },
            { MantleWebLocalizableStrings.Plugins.Model.LimitedToTenants, "Limited to Tenants" },

            { MantleWebLocalizableStrings.ScheduledTasks.ExecutedTaskError, "There was an error when executing the task." },
            { MantleWebLocalizableStrings.ScheduledTasks.ExecutedTaskSuccess, "Successfully executed task." },
            { MantleWebLocalizableStrings.ScheduledTasks.ManageScheduledTasks, "Manage Scheduled Tasks" },
            { MantleWebLocalizableStrings.ScheduledTasks.RunNow, "Run Now" },
            { MantleWebLocalizableStrings.ScheduledTasks.Model.Enabled, "Is Enabled" },
            { MantleWebLocalizableStrings.ScheduledTasks.Model.LastEndUtc, "Last End (UTC)" },
            { MantleWebLocalizableStrings.ScheduledTasks.Model.LastStartUtc, "Last Start (UTC)" },
            { MantleWebLocalizableStrings.ScheduledTasks.Model.LastSuccessUtc, "Last Success (UTC)" },
            { MantleWebLocalizableStrings.ScheduledTasks.Model.Name, "Name" },
            { MantleWebLocalizableStrings.ScheduledTasks.Model.Seconds, "Seconds" },
            { MantleWebLocalizableStrings.ScheduledTasks.Model.StopOnError, "Stop On Error" },
            { MantleWebLocalizableStrings.ScheduledTasks.Title, "Scheduled Tasks" },

            { MantleWebLocalizableStrings.Settings.Membership.DisallowUnconfirmedUserLogin, "Disallow Unconfirmed User Login" },
            { MantleWebLocalizableStrings.Settings.Membership.GeneratedPasswordLength, "Length" },
            { MantleWebLocalizableStrings.Settings.Membership.GeneratedPasswordNumberOfNonAlphanumericChars, "# Non-Alphanumeric Characters" },

            { MantleWebLocalizableStrings.Settings.Model.Name, "Name" },

            { MantleWebLocalizableStrings.Settings.Site.AdminLayoutPath, "Admin Layout Path" },
            { MantleWebLocalizableStrings.Settings.Site.AllowUserToSelectTheme, "Allow User To Select Theme" },
            { MantleWebLocalizableStrings.Settings.Site.DefaultFrontendLayoutPath, "Default Frontend Layout Path" },
            { MantleWebLocalizableStrings.Settings.Site.DefaultGridPageSize, "Default Grid Page Size" },
            { MantleWebLocalizableStrings.Settings.Site.DefaultLanguage, "Default Language" },
            { MantleWebLocalizableStrings.Settings.Site.DefaultMetaDescription, "Default Meta Description" },
            { MantleWebLocalizableStrings.Settings.Site.DefaultMetaKeywords, "Default Meta Keywords" },
            { MantleWebLocalizableStrings.Settings.Site.DefaultTheme, "Default Theme" },
            { MantleWebLocalizableStrings.Settings.Site.HomePageTitle, "Home Page Title" },
            { MantleWebLocalizableStrings.Settings.Site.SiteName, "Site Name" },
            { MantleWebLocalizableStrings.Settings.Site.Tabs.General, "General" },
            { MantleWebLocalizableStrings.Settings.Site.Tabs.Localization, "Localization" },
            { MantleWebLocalizableStrings.Settings.Site.Tabs.SEO, "SEO" },
            { MantleWebLocalizableStrings.Settings.Site.Tabs.Themes, "Themes" },

            { MantleWebLocalizableStrings.Tenants.ManageTenants, "Manage Tenants" },
            { MantleWebLocalizableStrings.Tenants.Model.Hosts, "Hosts" },
            { MantleWebLocalizableStrings.Tenants.Model.Url, "URL" },
            { MantleWebLocalizableStrings.Tenants.Title, "Tenants" },

            { MantleWebLocalizableStrings.Themes.Model.IsDefaultTheme, "Default Theme" },
            { MantleWebLocalizableStrings.Themes.Model.PreviewImageUrl, "Preview" },
            { MantleWebLocalizableStrings.Themes.Model.SupportRtl, "Supports RTL" },
            { MantleWebLocalizableStrings.Themes.SetDesktopThemeError, "Error when setting default desktop theme." },
            { MantleWebLocalizableStrings.Themes.SetDesktopThemeSuccess, "Successfully set default desktop theme." },
            { MantleWebLocalizableStrings.Themes.SetMobileThemeError, "Error when setting default mobile theme." },
            { MantleWebLocalizableStrings.Themes.SetMobileThemeSuccess, "Successfully set default mobile theme." },

            { MantleWebLocalizableStrings.UserProfile.Account.FamilyName, "Family Name" },
            { MantleWebLocalizableStrings.UserProfile.Account.GivenNames, "Given Name/s" },
            { MantleWebLocalizableStrings.UserProfile.Account.ShowFamilyNameFirst, "Show Family Name First" },
            { MantleWebLocalizableStrings.UserProfile.Localization.PreferredLanguage, "Preferred Language" },
            { MantleWebLocalizableStrings.UserProfile.Mobile.DontUseMobileVersion, "Don't Use Mobile Version" },
            { MantleWebLocalizableStrings.UserProfile.Theme.PreferredTheme, "Preferred Theme" },
        };

        #endregion ILanguagePack Members
    }
}