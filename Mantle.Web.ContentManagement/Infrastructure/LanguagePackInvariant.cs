﻿using Mantle.Localization;

namespace Mantle.Web.ContentManagement.Infrastructure;

public class LanguagePackInvariant : ILanguagePack
{
    #region ILanguagePack Members

    public string CultureCode => null;

    public IDictionary<string, string> LocalizedStrings => new Dictionary<string, string>
    {
        { MantleCmsLocalizableStrings.Blog.Categories, "Categories" },
        { MantleCmsLocalizableStrings.Blog.ManageBlog, "Manage Blog" },
        { MantleCmsLocalizableStrings.Blog.CategoryModel.Name, "Name" },
        { MantleCmsLocalizableStrings.Blog.CategoryModel.UrlSlug, "URL Slug" },
        { MantleCmsLocalizableStrings.Blog.PostModel.CategoryId, "Category" },
        { MantleCmsLocalizableStrings.Blog.PostModel.DateCreatedUtc, "Date Created (UTC)" },
        { MantleCmsLocalizableStrings.Blog.PostModel.ExternalLink, "External Link" },
        { MantleCmsLocalizableStrings.Blog.PostModel.FullDescription, "Full Description" },
        { MantleCmsLocalizableStrings.Blog.PostModel.Headline, "Headline" },
        { MantleCmsLocalizableStrings.Blog.PostModel.MetaDescription, "Meta Description" },
        { MantleCmsLocalizableStrings.Blog.PostModel.MetaKeywords, "Meta Keywords" },
        { MantleCmsLocalizableStrings.Blog.PostModel.ShortDescription, "Short Description" },
        { MantleCmsLocalizableStrings.Blog.PostModel.Slug, "URL Slug" },
        { MantleCmsLocalizableStrings.Blog.PostModel.TeaserImageUrl, "Teaser Image URL" },
        { MantleCmsLocalizableStrings.Blog.PostModel.UseExternalLink, "Use External Link" },
        { MantleCmsLocalizableStrings.Blog.TagModel.Name, "Name" },
        { MantleCmsLocalizableStrings.Blog.TagModel.UrlSlug, "URL Slug" },
        { MantleCmsLocalizableStrings.Blog.PostedByXOnX, "Posted by {0} on {1}." },
        { MantleCmsLocalizableStrings.Blog.Posts, "Posts" },
        { MantleCmsLocalizableStrings.Blog.Tags, "Tags" },
        { MantleCmsLocalizableStrings.Blog.Title, "Blog" },
        { MantleCmsLocalizableStrings.ContentBlocks.AllPostsBlock.CategoryId, "Category" },
        { MantleCmsLocalizableStrings.ContentBlocks.AllPostsBlock.FilterType, "Filter Type" },
        { MantleCmsLocalizableStrings.ContentBlocks.AllPostsBlock.TagId, "Tag" },
        { MantleCmsLocalizableStrings.ContentBlocks.CategoriesBlock.NumberOfCategories, "Number of Categories" },
        { MantleCmsLocalizableStrings.ContentBlocks.FormBlock.EmailAddress, "Email Address" },
        { MantleCmsLocalizableStrings.ContentBlocks.FormBlock.FormUrl, "Form URL" },
        { MantleCmsLocalizableStrings.ContentBlocks.FormBlock.HtmlTemplate, "HTML Template" },
        { MantleCmsLocalizableStrings.ContentBlocks.FormBlock.PleaseEnterCaptcha, "Please enter captcha validation field." },
        { MantleCmsLocalizableStrings.ContentBlocks.FormBlock.PleaseEnterCorrectCaptcha, "Please enter correct captcha validation field." },
        { MantleCmsLocalizableStrings.ContentBlocks.FormBlock.RedirectUrl, "Redirect URL (After Submit)" },
        { MantleCmsLocalizableStrings.ContentBlocks.FormBlock.SaveResultIfNotRedirectPleaseClick, "If this page does not automatically redirect, please click the following link to continue:" },
        { MantleCmsLocalizableStrings.ContentBlocks.FormBlock.SaveResultRedirect, "Redirect" },
        { MantleCmsLocalizableStrings.ContentBlocks.FormBlock.ThankYouMessage, "'Thank You' Message" },
        { MantleCmsLocalizableStrings.ContentBlocks.FormBlock.UseAjax, "Use Ajax" },
        { MantleCmsLocalizableStrings.ContentBlocks.HtmlBlock.BodyContent, "Body Content" },
        { MantleCmsLocalizableStrings.ContentBlocks.HtmlBlock.HelpText.BodyContent, "The HTML content to display." },
        { MantleCmsLocalizableStrings.ContentBlocks.HtmlBlock.HelpText.Script, "Optional JavaScript to add to the page." },
        { MantleCmsLocalizableStrings.ContentBlocks.HtmlBlock.Script, "Script" },
        { MantleCmsLocalizableStrings.ContentBlocks.FormBlock.HelpText.EmailAddress, "The email address to send the form values to." },
        { MantleCmsLocalizableStrings.ContentBlocks.FormBlock.HelpText.FormUrl, "Specify URL to send the form values to or leave blank for default behavior (form values are emailed to the specified address)." },
        { MantleCmsLocalizableStrings.ContentBlocks.LanguageSwitchBlock.CustomTemplatePath, "Custom Template Path" },
        { MantleCmsLocalizableStrings.ContentBlocks.LanguageSwitchBlock.IncludeInvariant, "Include Invariant" },
        { MantleCmsLocalizableStrings.ContentBlocks.LanguageSwitchBlock.InvariantText, "Invariant Text" },
        { MantleCmsLocalizableStrings.ContentBlocks.LanguageSwitchBlock.Style, "Style" },
        { MantleCmsLocalizableStrings.ContentBlocks.LanguageSwitchBlock.UseUrlPrefix, "Use URL Prefix" },
        { MantleCmsLocalizableStrings.ContentBlocks.LastNPostsBlock.NumberOfEntries, "# of Entries" },
        { MantleCmsLocalizableStrings.ContentBlocks.ManageContentBlocks, "Manage Content Blocks" },
        { MantleCmsLocalizableStrings.ContentBlocks.ManageZones, "Manage Zones" },
        { MantleCmsLocalizableStrings.ContentBlocks.Model.BlockType, "Block Type" },
        { MantleCmsLocalizableStrings.ContentBlocks.Model.CustomTemplatePath, "Custom Template Path" },
        { MantleCmsLocalizableStrings.ContentBlocks.Model.IsEnabled, "Is Enabled" },
        { MantleCmsLocalizableStrings.ContentBlocks.Model.Order, "Order" },
        { MantleCmsLocalizableStrings.ContentBlocks.Model.Title, "Title" },
        { MantleCmsLocalizableStrings.ContentBlocks.Model.ZoneId, "Zone" },
        { MantleCmsLocalizableStrings.ContentBlocks.NewsletterSubscriptionBlock.Email, "Email" },
        { MantleCmsLocalizableStrings.ContentBlocks.NewsletterSubscriptionBlock.EmailPlaceholder, "Your E-Mail Address" },
        { MantleCmsLocalizableStrings.ContentBlocks.NewsletterSubscriptionBlock.Name, "Name" },
        { MantleCmsLocalizableStrings.ContentBlocks.NewsletterSubscriptionBlock.NamePlaceholder, "Your Name" },
        { MantleCmsLocalizableStrings.ContentBlocks.NewsletterSubscriptionBlock.SignUpLabel, "Sign up for newsletters" },
        { MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.AfterCloudRender, "After Cloud Render" },
        { MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.AutoResize, "Auto Resize" },
        //{ MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.CenterX, "Center X" },
        //{ MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.CenterY, "Center Y" },
        { MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.ClassPattern, "Class Pattern" },
        { MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.Colors, "Colors" },
        { MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.Delay, "Delay" },
        { MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.EncodeURI, "Encode URI" },
        { MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.FontSizeFrom, "Font Size From" },
        { MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.FontSizeTo, "Font Size To" },
        //{ MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.Height, "Height" },
        //{ MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.HeightUnit, "Height Unit" },
        { MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.RemoveOverflowing, "Remove Overflowing" },
        { MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.Shape, "Shape" },
        { MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.Steps, "Steps" },
        //{ MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.Width, "Width" },
        //{ MantleCmsLocalizableStrings.ContentBlocks.TagCloudBlock.WidthUnit, "Width Unit" },
        { MantleCmsLocalizableStrings.ContentBlocks.Title, "Content Blocks" },
        { MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.AutoPlay, "Auto Play" },
        { MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.ControlId, "Control ID" },
        { MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.Loop, "Loop" },
        { MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.ShowControls, "Show Controls" },
        { MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.Source, "Source" },
        { MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.Type, "Type" },
        { MantleCmsLocalizableStrings.ContentBlocks.VideoBlock.VideoTagNotSupported, "Your browser is too old to support HTML5 Video. Please update your browser." },
        { MantleCmsLocalizableStrings.ContentBlocks.ZoneModel.Name, "Name" },
        { MantleCmsLocalizableStrings.ContentBlocks.Zones, "Zones" },
        { MantleCmsLocalizableStrings.Localization.IsRTL, "Is Right-to-Left" },
        { MantleCmsLocalizableStrings.Localization.LanguageModel.CultureCode, "Culture" },
        { MantleCmsLocalizableStrings.Localization.LanguageModel.IsEnabled, "Enabled" },
        { MantleCmsLocalizableStrings.Localization.LanguageModel.IsRTL, "Right-to-Left" },
        { MantleCmsLocalizableStrings.Localization.LanguageModel.Name, "Name" },
        { MantleCmsLocalizableStrings.Localization.LanguageModel.SortOrder, "Sort Order" },
        { MantleCmsLocalizableStrings.Localization.Languages, "Languages" },
        { MantleCmsLocalizableStrings.Localization.LocalizableStringModel.InvariantValue, "Invariant" },
        { MantleCmsLocalizableStrings.Localization.LocalizableStringModel.Key, "Key" },
        { MantleCmsLocalizableStrings.Localization.LocalizableStringModel.LocalizedValue, "Localized" },
        { MantleCmsLocalizableStrings.Localization.LocalizableStrings, "Localizable Strings" },
        { MantleCmsLocalizableStrings.Localization.Localize, "Localize" },
        { MantleCmsLocalizableStrings.Localization.ManageLanguages, "Manage Languages" },
        { MantleCmsLocalizableStrings.Localization.ManageLocalizableStrings, "Manage Localizable Strings" },
        { MantleCmsLocalizableStrings.Localization.SelectLanguage, "Select Language" },
        { MantleCmsLocalizableStrings.Localization.Title, "Localization" },
        { MantleCmsLocalizableStrings.Localization.Translate, "Translate" },
        { MantleCmsLocalizableStrings.Localization.Translations, "Translations" },
        { MantleCmsLocalizableStrings.Media.FileBrowserTitle, "File Browser" },
        { MantleCmsLocalizableStrings.Media.ManageMedia, "Manage Media" },
        { MantleCmsLocalizableStrings.Media.Title, "Media" },
        { MantleCmsLocalizableStrings.Menus.IsExternalUrl, "Is External Url" },
        { MantleCmsLocalizableStrings.Menus.Items, "Items" },
        { MantleCmsLocalizableStrings.Menus.ManageMenuItems, "Manage Menu Items" },
        { MantleCmsLocalizableStrings.Menus.ManageMenus, "Manage Menus" },
        { MantleCmsLocalizableStrings.Menus.MenuItemModel.CssClass, "CSS Class" },
        { MantleCmsLocalizableStrings.Menus.MenuItemModel.Description, "Description" },
        { MantleCmsLocalizableStrings.Menus.MenuItemModel.Enabled, "Enabled" },
        { MantleCmsLocalizableStrings.Menus.MenuItemModel.Position, "Position" },
        { MantleCmsLocalizableStrings.Menus.MenuItemModel.Text, "Text" },
        { MantleCmsLocalizableStrings.Menus.MenuItemModel.Url, "Url" },
        { MantleCmsLocalizableStrings.Menus.MenuItems, "Menu Items" },
        { MantleCmsLocalizableStrings.Menus.MenuModel.Name, "Name" },
        { MantleCmsLocalizableStrings.Menus.MenuModel.UrlFilter, "URL Filter" },
        { MantleCmsLocalizableStrings.Menus.NewItem, "New Item" },
        { MantleCmsLocalizableStrings.Menus.Title, "Menus" },
        { MantleCmsLocalizableStrings.Messages.CircularRelationshipError, "That action would cause a circular relationship!" },
        { MantleCmsLocalizableStrings.Messages.ConfirmClearLocalizableStrings, "Warning! This will remove all localized strings from the database. Are you sure you want to do this?" },
        { MantleCmsLocalizableStrings.Messages.GetTranslationError, "There was an error when retrieving the translation." },
        { MantleCmsLocalizableStrings.Messages.UpdateTranslationError, "There was an error when saving the translation." },
        { MantleCmsLocalizableStrings.Messages.UpdateTranslationSuccess, "Successfully saved translation." },
        { MantleCmsLocalizableStrings.Navigation.CMS, "CMS" },
        { MantleCmsLocalizableStrings.Newsletters.ExportToCSV, "Export To CSV" },
        { MantleCmsLocalizableStrings.Newsletters.Subscribers, "Subscribers" },
        { MantleCmsLocalizableStrings.Newsletters.SuccessfullySignedUp, "You have successfully signed up for newsletters." },
        { MantleCmsLocalizableStrings.Newsletters.Title, "Newsletters" },
        { MantleCmsLocalizableStrings.Pages.CannotDeleteOnlyVersion, "Cannot delete the only version of a page. Create a new version first." },
        { MantleCmsLocalizableStrings.Pages.History, "History" },
        { MantleCmsLocalizableStrings.Pages.ManagePages, "Manage Pages" },
        { MantleCmsLocalizableStrings.Pages.Page, "Page" },
        { MantleCmsLocalizableStrings.Pages.PageHistory, "Page History" },
        { MantleCmsLocalizableStrings.Pages.PageHistoryRestoreConfirm, "Are you sure you want to restore this version?" },
        { MantleCmsLocalizableStrings.Pages.PageHistoryRestoreError, "There was an error when trying to restore the specified page version." },
        { MantleCmsLocalizableStrings.Pages.PageHistoryRestoreSuccess, "Successfully restored specified page version." },
        { MantleCmsLocalizableStrings.Pages.PageModel.IsEnabled, "Is Enabled" },
        { MantleCmsLocalizableStrings.Pages.PageModel.Name, "Name" },
        { MantleCmsLocalizableStrings.Pages.PageModel.Order, "Order" },
        { MantleCmsLocalizableStrings.Pages.PageModel.PageTypeId, "Page Type" },
        { MantleCmsLocalizableStrings.Pages.PageModel.Roles, "Roles" },
        { MantleCmsLocalizableStrings.Pages.PageModel.ShowOnMenus, "Show on Menus" },
        { MantleCmsLocalizableStrings.Pages.PageVersionModel.CultureCode, "Culture Code" },
        { MantleCmsLocalizableStrings.Pages.PageVersionModel.DateCreated, "Date Created (UTC)" },
        { MantleCmsLocalizableStrings.Pages.PageVersionModel.DateModified, "Date Modified (UTC)" },
        { MantleCmsLocalizableStrings.Pages.PageVersionModel.IsDraft, "Is Draft" },
        { MantleCmsLocalizableStrings.Pages.PageVersionModel.Slug, "URL Slug" },
        { MantleCmsLocalizableStrings.Pages.PageVersionModel.Status, "Status" },
        { MantleCmsLocalizableStrings.Pages.PageVersionModel.Title, "Title" },
        { MantleCmsLocalizableStrings.Pages.PageTypeModel.DisplayTemplatePath, "Display Template Path" },
        { MantleCmsLocalizableStrings.Pages.PageTypeModel.EditorTemplatePath, "Editor Template Path" },
        { MantleCmsLocalizableStrings.Pages.PageTypeModel.LayoutPath, "Layout Path" },
        { MantleCmsLocalizableStrings.Pages.PageTypeModel.Name, "Name" },
        { MantleCmsLocalizableStrings.Pages.PageTypes.StandardPage.BodyContent, "Body Content" },
        { MantleCmsLocalizableStrings.Pages.PageTypes.StandardPage.MetaDescription, "Meta Description" },
        { MantleCmsLocalizableStrings.Pages.PageTypes.StandardPage.MetaKeywords, "Meta Keywords" },
        { MantleCmsLocalizableStrings.Pages.PageTypes.StandardPage.MetaTitle, "Meta Title" },
        { MantleCmsLocalizableStrings.Pages.PageTypes.Title, "Page Types" },
        { MantleCmsLocalizableStrings.Pages.Restore, "Restore" },
        { MantleCmsLocalizableStrings.Pages.RestoreVersion, "Restore Version" },
        { MantleCmsLocalizableStrings.Pages.SelectPageToBeginEdit, "Select a page to begin editing." },
        { MantleCmsLocalizableStrings.Pages.Tags, "Tags" },
        { MantleCmsLocalizableStrings.Pages.Title, "Pages" },
        { MantleCmsLocalizableStrings.Pages.Translations, "Translations" },
        { MantleCmsLocalizableStrings.Pages.Versions, "Versions" },
        { MantleCmsLocalizableStrings.Settings.Blog.AccessRestrictions, "Access Restrictions" },
        { MantleCmsLocalizableStrings.Settings.Blog.DateFormat, "Date Format" },
        { MantleCmsLocalizableStrings.Settings.Blog.ItemsPerPage, "# Items Per Page" },
        { MantleCmsLocalizableStrings.Settings.Blog.MenuPosition, "Menu Position" },
        { MantleCmsLocalizableStrings.Settings.Blog.PageTitle, "Page Title" },
        { MantleCmsLocalizableStrings.Settings.Blog.ShowOnMenus, "Show on Menus" },
        { MantleCmsLocalizableStrings.Settings.Blog.LayoutPathOverride, "Layout Path (Override)" },
        { MantleCmsLocalizableStrings.Settings.Pages.NumberOfPageVersionsToKeep, "# Page Versions to Keep" },
        { MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequency, "Change Frequency" },
        { MantleCmsLocalizableStrings.Sitemap.Model.Id, "ID" },
        { MantleCmsLocalizableStrings.Sitemap.Model.Location, "Location" },
        { MantleCmsLocalizableStrings.Sitemap.Model.Priority, "Priority" },
        { MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Always, "Always" },
        { MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Daily, "Daily" },
        { MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Hourly, "Hourly" },
        { MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Monthly, "Monthly" },
        { MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Never, "Never" },
        { MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Weekly, "Weekly" },
        { MantleCmsLocalizableStrings.Sitemap.Model.ChangeFrequencies.Yearly, "Yearly" },
        { MantleCmsLocalizableStrings.Sitemap.ConfirmGenerateFile, "Are you sure you want to generate a new XML sitemap file? Warning: This will replace the existing one." },
        { MantleCmsLocalizableStrings.Sitemap.GenerateFile, "Generate File" },
        { MantleCmsLocalizableStrings.Sitemap.GenerateFileError, "Error when generating XML sitemap file." },
        { MantleCmsLocalizableStrings.Sitemap.GenerateFileSuccess, "Successfully generated XML sitemap file." },
        { MantleCmsLocalizableStrings.Sitemap.Title, "Sitemap" },
        { MantleCmsLocalizableStrings.Sitemap.XMLSitemap, "XML Sitemap" },
        { MantleCmsLocalizableStrings.UserProfile.Newsletter.SubscribeToNewsletters, "Subscribe to Newsletters" },
    };

    #endregion ILanguagePack Members
}