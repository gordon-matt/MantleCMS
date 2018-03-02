namespace Mantle.Web.ContentManagement
{
    public static class MantleCmsLocalizableStrings
    {
        public static class Blog
        {
            public const string Categories = "Mantle.Web.ContentManagement/Blog.Categories";
            public const string ManageBlog = "Mantle.Web.ContentManagement/Blog.ManageBlog";
            public const string PostedByXOnX = "Mantle.Web.ContentManagement/Blog.PostedByXOnX";
            public const string Posts = "Mantle.Web.ContentManagement/Blog.Posts";
            public const string Tags = "Mantle.Web.ContentManagement/Blog.Tags";
            public const string Title = "Mantle.Web.ContentManagement/Blog.Title";

            public static class CategoryModel
            {
                public const string Name = "Mantle.Web.ContentManagement/Blog.CategoryModel.Name";
                public const string UrlSlug = "Mantle.Web.ContentManagement/Blog.CategoryModel.UrlSlug";
            }

            public static class PostModel
            {
                public const string CategoryId = "Mantle.Web.ContentManagement/Blog.PostModel.CategoryId";
                public const string DateCreatedUtc = "Mantle.Web.ContentManagement/Blog.PostModel.DateCreatedUtc";
                public const string ExternalLink = "Mantle.Web.ContentManagement/Blog.PostModel.ExternalLink";
                public const string FullDescription = "Mantle.Web.ContentManagement/Blog.PostModel.FullDescription";
                public const string Headline = "Mantle.Web.ContentManagement/Blog.PostModel.Headline";
                public const string MetaDescription = "Mantle.Web.ContentManagement/Blog.PostModel.MetaDescription";
                public const string MetaKeywords = "Mantle.Web.ContentManagement/Blog.PostModel.MetaKeywords";
                public const string ShortDescription = "Mantle.Web.ContentManagement/Blog.PostModel.ShortDescription";
                public const string Slug = "Mantle.Web.ContentManagement/Blog.PostModel.Slug";
                public const string TeaserImageUrl = "Mantle.Web.ContentManagement/Blog.PostModel.TeaserImageUrl";
                public const string UseExternalLink = "Mantle.Web.ContentManagement/Blog.PostModel.UseExternalLink";
            }

            public static class TagModel
            {
                public const string Name = "Mantle.Web.ContentManagement/Blog.TagModel.Name";
                public const string UrlSlug = "Mantle.Web.ContentManagement/Blog.TagModel.UrlSlug";
            }
        }

        public static class ContentBlocks
        {
            public const string ManageContentBlocks = "Mantle.Web.ContentManagement/ContentBlocks.ManageContentBlocks";
            public const string ManageZones = "Mantle.Web.ContentManagement/ContentBlocks.ManageZones";
            public const string Title = "Mantle.Web.ContentManagement/ContentBlocks.Title";
            public const string Zones = "Mantle.Web.ContentManagement/ContentBlocks.Zones";

            #region Blog

            public static class AllPostsBlock
            {
                public const string CategoryId = "Mantle.Web.ContentManagement/ContentBlocks.AllPostsBlock.CategoryId";
                public const string FilterType = "Mantle.Web.ContentManagement/ContentBlocks.AllPostsBlock.FilterType";
                public const string TagId = "Mantle.Web.ContentManagement/ContentBlocks.AllPostsBlock.TagId";
            }

            public static class CategoriesBlock
            {
                public const string NumberOfCategories = "Mantle.Web.ContentManagement/ContentBlocks.CategoriesBlock.NumberOfCategories";
            }

            public static class LastNPostsBlock
            {
                public const string NumberOfEntries = "Mantle.Web.ContentManagement/ContentBlocks.LastNPostsBlock.NumberOfEntries";
            }

            public static class TagCloudBlock
            {
                //public const string Width = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.Width";
                //public const string WidthUnit = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.WidthUnit";
                //public const string Height = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.Height";
                //public const string HeightUnit = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.HeightUnit";
                //public const string CenterX = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.CenterX";
                //public const string CenterY = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.CenterY";
                public const string AutoResize = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.AutoResize";

                public const string Steps = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.Steps";
                public const string ClassPattern = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.ClassPattern";
                public const string AfterCloudRender = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.AfterCloudRender";
                public const string Delay = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.Delay";
                public const string Shape = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.Shape";
                public const string RemoveOverflowing = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.RemoveOverflowing";
                public const string EncodeURI = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.EncodeURI";
                public const string Colors = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.Colors";
                public const string FontSizeFrom = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.FontSizeFrom";
                public const string FontSizeTo = "Mantle.Web.ContentManagement/ContentBlocks.TagCloudBlock.FontSizeTo";
            }

            #endregion Blog

            public static class FormBlock
            {
                public static class HelpText
                {
                    public const string EmailAddress = "Mantle.Web.ContentManagement/ContentBlocks.FormBlock.HelpText.EmailAddress";
                    public const string FormUrl = "Mantle.Web.ContentManagement/ContentBlocks.FormBlock.HelpText.FormUrl";
                }

                public const string EmailAddress = "Mantle.Web.ContentManagement/ContentBlocks.FormBlock.EmailAddress";
                public const string FormUrl = "Mantle.Web.ContentManagement/ContentBlocks.FormBlock.FormUrl";
                public const string HtmlTemplate = "Mantle.Web.ContentManagement/ContentBlocks.FormBlock.HtmlTemplate";
                public const string PleaseEnterCaptcha = "Mantle.Web.ContentManagement/ContentBlocks.FormBlock.PleaseEnterCaptcha";
                public const string PleaseEnterCorrectCaptcha = "Mantle.Web.ContentManagement/ContentBlocks.FormBlock.PleaseEnterCorrectCaptcha";
                public const string RedirectUrl = "Mantle.Web.ContentManagement/ContentBlocks.FormBlock.RedirectUrl";
                public const string SaveResultIfNotRedirectPleaseClick = "Mantle.Web.ContentManagement/ContentBlocks.FormBlock.SaveResultIfNotRedirectPleaseClick";
                public const string SaveResultRedirect = "Mantle.Web.ContentManagement/ContentBlocks.FormBlock.SaveResultRedirect";
                public const string ThankYouMessage = "Mantle.Web.ContentManagement/ContentBlocks.FormBlock.ThankYouMessage";
                public const string UseAjax = "Mantle.Web.ContentManagement/ContentBlocks.FormBlock.UseAjax";
            }

            public static class HtmlBlock
            {
                public static class HelpText
                {
                    public const string BodyContent = "Mantle.Web.ContentManagement/ContentBlocks.HtmlBlock.HelpText.BodyContent";
                    public const string Script = "Mantle.Web.ContentManagement/ContentBlocks.HtmlBlock.HelpText.Script";
                }

                public const string BodyContent = "Mantle.Web.ContentManagement/ContentBlocks.HtmlBlock.BodyContent";
                public const string Script = "Mantle.Web.ContentManagement/ContentBlocks.HtmlBlock.Script";
            }

            public static class LanguageSwitchBlock
            {
                public const string CustomTemplatePath = "Mantle.Web.ContentManagement/ContentBlocks.LanguageSwitchBlock.CustomTemplatePath";
                public const string IncludeInvariant = "Mantle.Web.ContentManagement/ContentBlocks.LanguageSwitchBlock.IncludeInvariant";
                public const string InvariantText = "Mantle.Web.ContentManagement/ContentBlocks.LanguageSwitchBlock.InvariantText";
                public const string Style = "Mantle.Web.ContentManagement/ContentBlocks.LanguageSwitchBlock.Style";
                public const string UseUrlPrefix = "Mantle.Web.ContentManagement/ContentBlocks.LanguageSwitchBlock.UseUrlPrefix";
            }

            public static class Model
            {
                public const string BlockType = "Mantle.Web.ContentManagement/ContentBlocks.Model.BlockType";
                public const string CustomTemplatePath = "Mantle.Web.ContentManagement/ContentBlocks.Model.CustomTemplatePath";
                public const string DisplayCondition = "Mantle.Web.ContentManagement/ContentBlocks.Model.DisplayCondition";
                public const string IsEnabled = "Mantle.Web.ContentManagement/ContentBlocks.Model.IsEnabled";
                public const string Order = "Mantle.Web.ContentManagement/ContentBlocks.Model.Order";
                public const string Title = "Mantle.Web.ContentManagement/ContentBlocks.Model.Title";
                public const string ZoneId = "Mantle.Web.ContentManagement/ContentBlocks.Model.ZoneId";
            }

            public static class NewsletterSubscriptionBlock
            {
                public const string Email = "Mantle.Web.ContentManagement/ContentBlocks.NewsletterSubscriptionBlock.Email";
                public const string EmailPlaceholder = "Mantle.Web.ContentManagement/ContentBlocks.NewsletterSubscriptionBlock.EmailPlaceholder";
                public const string Name = "Mantle.Web.ContentManagement/ContentBlocks.NewsletterSubscriptionBlock.Name";
                public const string NamePlaceholder = "Mantle.Web.ContentManagement/ContentBlocks.NewsletterSubscriptionBlock.NamePlaceholder";
                public const string SignUpLabel = "Mantle.Web.ContentManagement/ContentBlocks.NewsletterSubscriptionBlock.SignUpLabel";
            }

            public static class VideoBlock
            {
                public const string ControlId = "Mantle.Web.ContentManagement/ContentBlocks.VideoBlock.ControlId";
                public const string Type = "Mantle.Web.ContentManagement/ContentBlocks.VideoBlock.Type";
                public const string Source = "Mantle.Web.ContentManagement/ContentBlocks.VideoBlock.Source";
                public const string ShowControls = "Mantle.Web.ContentManagement/ContentBlocks.VideoBlock.ShowControls";
                public const string AutoPlay = "Mantle.Web.ContentManagement/ContentBlocks.VideoBlock.AutoPlay";
                public const string Loop = "Mantle.Web.ContentManagement/ContentBlocks.VideoBlock.Loop";
                public const string VideoTagNotSupported = "Mantle.Web.ContentManagement/ContentBlocks.VideoBlock.VideoTagNotSupported";
            }

            public static class ZoneModel
            {
                public const string Name = "Mantle.Web.ContentManagement/ContentBlocks.ZoneModel.Name";
            }
        }

        public static class Localization
        {
            public const string IsRTL = "Mantle.Web.ContentManagement/Localization.IsRTL";
            public const string Languages = "Mantle.Web.ContentManagement/Localization.Languages";
            public const string LocalizableStrings = "Mantle.Web.ContentManagement/Localization.LocalizableStrings";
            public const string Localize = "Mantle.Web.ContentManagement/Localization.Localize";
            public const string ManageLanguages = "Mantle.Web.ContentManagement/Localization.ManageLanguages";
            public const string ManageLocalizableStrings = "Mantle.Web.ContentManagement/Localization.ManageLocalizableStrings";
            public const string SelectLanguage = "Mantle.Web.ContentManagement/Localization.SelectLanguage";
            public const string Title = "Mantle.Web.ContentManagement/Localization.Title";
            public const string Translate = "Mantle.Web.ContentManagement/Localization.Translate";
            public const string Translations = "Mantle.Web.ContentManagement/Localization.Translations";

            public static class LanguageModel
            {
                public const string CultureCode = "Mantle.Web.ContentManagement/Localization.LanguageModel.CultureCode";
                public const string IsEnabled = "Mantle.Web.ContentManagement/Localization.LanguageModel.IsEnabled";
                public const string IsRTL = "Mantle.Web.ContentManagement/Localization.LanguageModel.IsRTL";
                public const string Name = "Mantle.Web.ContentManagement/Localization.LanguageModel.Name";
                public const string SortOrder = "Mantle.Web.ContentManagement/Localization.LanguageModel.SortOrder";
            }

            public static class LocalizableStringModel
            {
                public const string InvariantValue = "Mantle.Web.ContentManagement/Localization.LocalizableStringModel.InvariantValue";
                public const string Key = "Mantle.Web.ContentManagement/Localization.LocalizableStringModel.Key";
                public const string LocalizedValue = "Mantle.Web.ContentManagement/Localization.LocalizableStringModel.LocalizedValue";
            }
        }

        public static class Media
        {
            public const string FileBrowserTitle = "Mantle.Web.ContentManagement/Media.FileBrowserTitle";
            public const string ManageMedia = "Mantle.Web.ContentManagement/Media.ManageMedia";
            public const string Title = "Mantle.Web.ContentManagement/Media.Title";
        }

        public static class Menus
        {
            public const string IsExternalUrl = "Mantle.Web.ContentManagement/Menus.IsExternalUrl";
            public const string Items = "Mantle.Web.ContentManagement/Menus.Items";
            public const string ManageMenuItems = "Mantle.Web.ContentManagement/Menus.ManageMenuItems";
            public const string ManageMenus = "Mantle.Web.ContentManagement/Menus.ManageMenus";
            public const string MenuItems = "Mantle.Web.ContentManagement/Menus.MenuItems";
            public const string NewItem = "Mantle.Web.ContentManagement/Menus.NewItem";
            public const string Title = "Mantle.Web.ContentManagement/Menus.Title";

            public static class MenuItemModel
            {
                public const string CssClass = "Mantle.Web.ContentManagement/Menus.MenuItemModel.CssClass";
                public const string Description = "Mantle.Web.ContentManagement/Menus.MenuItemModel.Description";
                public const string Enabled = "Mantle.Web.ContentManagement/Menus.MenuItemModel.Enabled";
                public const string Position = "Mantle.Web.ContentManagement/Menus.MenuItemModel.Position";
                public const string Text = "Mantle.Web.ContentManagement/Menus.MenuItemModel.Text";
                public const string Url = "Mantle.Web.ContentManagement/Menus.MenuItemModel.Url";
            }

            public static class MenuModel
            {
                public const string Name = "Mantle.Web.ContentManagement/Menus.MenuModel.Name";
                public const string UrlFilter = "Mantle.Web.ContentManagement/Menus.MenuModel.UrlFilter";
            }
        }

        public static class Messages
        {
            public const string CircularRelationshipError = "Mantle.Web.ContentManagement/Messages.CircularRelationshipError";
            public const string ConfirmClearLocalizableStrings = "Mantle.Web.ContentManagement/Messages.ConfirmClearLocalizableStrings";
            public const string GetTranslationError = "Mantle.Web.ContentManagement/Messages.GetTranslationError";
            public const string UpdateTranslationError = "Mantle.Web.ContentManagement/Messages.UpdateTranslationError";
            public const string UpdateTranslationSuccess = "Mantle.Web.ContentManagement/Messages.UpdateTranslationSuccess";
        }

        public static class Navigation
        {
            public const string CMS = "Mantle.Web.ContentManagement/Navigation.CMS";
        }

        public static class Newsletters
        {
            public const string ExportToCSV = "Mantle.Web.ContentManagement/Newsletters.ExportToCSV";
            public const string Subscribers = "Mantle.Web.ContentManagement/Newsletters.Subscribers";
            public const string SuccessfullySignedUp = "Mantle.Web.ContentManagement/Newsletters.SuccessfullySignedUp";
            public const string Title = "Mantle.Web.ContentManagement/Newsletters.Title";
        }

        public static class Pages
        {
            public const string ConfirmRestoreVersion = "Mantle.Web.ContentManagement/Pages.ConfirmRestoreVersion";
            public const string History = "Mantle.Web.ContentManagement/Pages.History";
            public const string ManagePages = "Mantle.Web.ContentManagement/Pages.ManagePages";
            public const string Page = "Mantle.Web.ContentManagement/Pages.Page";
            public const string PageHistory = "Mantle.Web.ContentManagement/Pages.PageHistory";
            public const string PageHistoryRestoreConfirm = "Mantle.Web.ContentManagement/Pages.PageHistoryRestoreConfirm";
            public const string PageHistoryRestoreError = "Mantle.Web.ContentManagement/Pages.PageHistoryRestoreError";
            public const string PageHistoryRestoreSuccess = "Mantle.Web.ContentManagement/Pages.PageHistoryRestoreSuccess";
            public const string Restore = "Mantle.Web.ContentManagement/Pages.Restore";
            public const string RestoreVersion = "Mantle.Web.ContentManagement/Pages.RestoreVersion";
            public const string SelectPageToBeginEdit = "Mantle.Web.ContentManagement/Pages.SelectPageToBeginEdit";
            public const string Tags = "Mantle.Web.ContentManagement/Pages.Tags";
            public const string Title = "Mantle.Web.ContentManagement/Pages.Title";
            public const string Translations = "Mantle.Web.ContentManagement/Pages.Translations";
            public const string Versions = "Mantle.Web.ContentManagement/Pages.Versions";

            public const string CannotDeleteOnlyVersion = "Mantle.Web.ContentManagement/Pages.CannotDeleteOnlyVersion";

            public static class PageModel
            {
                public const string IsEnabled = "Mantle.Web.ContentManagement/Pages.PageModel.IsEnabled";
                public const string Name = "Mantle.Web.ContentManagement/Pages.PageModel.Name";
                public const string Order = "Mantle.Web.ContentManagement/Pages.PageModel.Order";
                public const string PageTypeId = "Mantle.Web.ContentManagement/Pages.PageModel.PageTypeId";
                public const string Roles = "Mantle.Web.ContentManagement/Pages.PageModel.Roles";
                public const string ShowOnMenus = "Mantle.Web.ContentManagement/Pages.PageModel.ShowOnMenus";
            }

            public static class PageVersionModel
            {
                public const string CultureCode = "Mantle.Web.ContentManagement/Pages.PageVersionModel.CultureCode";
                public const string DateCreated = "Mantle.Web.ContentManagement/Pages.PageVersionModel.DateCreated";
                public const string DateModified = "Mantle.Web.ContentManagement/Pages.PageVersionModel.DateModified";
                public const string IsDraft = "Mantle.Web.ContentManagement/Pages.PageVersionModel.IsDraft";
                public const string Slug = "Mantle.Web.ContentManagement/Pages.PageVersionModel.Slug";
                public const string Status = "Mantle.Web.ContentManagement/Pages.PageVersionModel.Status";
                public const string Title = "Mantle.Web.ContentManagement/Pages.PageVersionModel.Title";
            }

            public static class PageTypeModel
            {
                public const string DisplayTemplatePath = "Mantle.Web.ContentManagement/Pages.PageTypeModel.DisplayTemplatePath";
                public const string EditorTemplatePath = "Mantle.Web.ContentManagement/Pages.PageTypeModel.EditorTemplatePath";
                public const string LayoutPath = "Mantle.Web.ContentManagement/Pages.PageTypeModel.LayoutPath";
                public const string Name = "Mantle.Web.ContentManagement/Pages.PageTypeModel.Name";
            }

            public static class PageTypes
            {
                public const string Title = "Mantle.Web.ContentManagement/Pages.PageTypes.Title";

                public static class StandardPage
                {
                    public const string BodyContent = "Mantle.Web.ContentManagement/Pages.PageTypes.StandardPage.BodyContent";
                    public const string MetaDescription = "Mantle.Web.ContentManagement/Pages.PageTypes.StandardPage.MetaDescription";
                    public const string MetaKeywords = "Mantle.Web.ContentManagement/Pages.PageTypes.StandardPage.MetaKeywords";
                    public const string MetaTitle = "Mantle.Web.ContentManagement/Pages.PageTypes.StandardPage.MetaTitle";
                }
            }
        }

        public static class Settings
        {
            public static class Blog
            {
                public const string AccessRestrictions = "Mantle.Web.ContentManagement/Settings.Blog.AccessRestrictions";
                public const string DateFormat = "Mantle.Web.ContentManagement/Settings.Blog.DateFormat";
                public const string ItemsPerPage = "Mantle.Web.ContentManagement/Settings.Blog.ItemsPerPage";
                public const string MenuPosition = "Mantle.Web.ContentManagement/Settings.Blog.MenuPosition";
                public const string PageTitle = "Mantle.Web.ContentManagement/Settings.Blog.PageTitle";
                public const string ShowOnMenus = "Mantle.Web.ContentManagement/Settings.Blog.ShowOnMenus";
                public const string UseAjax = "Mantle.Web.ContentManagement/Settings.Blog.UseAjax";
                public const string LayoutPathOverride = "Mantle.Web.ContentManagement/Settings.Blog.LayoutPathOverride";
            }

            public static class Pages
            {
                public const string NumberOfPageVersionsToKeep = "Mantle.Web.ContentManagement/Settings.Pages.NumberOfPageVersionsToKeep";
            }
        }

        public static class Sitemap
        {
            public static class Model
            {
                public static class ChangeFrequencies
                {
                    public const string Always = "Mantle.Web.ContentManagement/Sitemap.Model.ChangeFrequencies.Always";
                    public const string Hourly = "Mantle.Web.ContentManagement/Sitemap.Model.ChangeFrequencies.Hourly";
                    public const string Daily = "Mantle.Web.ContentManagement/Sitemap.Model.ChangeFrequencies.Daily";
                    public const string Weekly = "Mantle.Web.ContentManagement/Sitemap.Model.ChangeFrequencies.Weekly";
                    public const string Monthly = "Mantle.Web.ContentManagement/Sitemap.Model.ChangeFrequencies.Monthly";
                    public const string Yearly = "Mantle.Web.ContentManagement/Sitemap.Model.ChangeFrequencies.Yearly";
                    public const string Never = "Mantle.Web.ContentManagement/Sitemap.Model.ChangeFrequencies.Never";
                }

                public const string ChangeFrequency = "Mantle.Web.ContentManagement/SitemapModel.ChangeFrequency";
                public const string Id = "Mantle.Web.ContentManagement/SitemapModel.Id";
                public const string Location = "Mantle.Web.ContentManagement/SitemapModel.Location";
                public const string Priority = "Mantle.Web.ContentManagement/SitemapModel.Priority";
            }

            public const string ConfirmGenerateFile = "Mantle.Web.ContentManagement/Sitemap.ConfirmGenerateFile";
            public const string GenerateFile = "Mantle.Web.ContentManagement/Sitemap.GenerateFile";
            public const string GenerateFileError = "Mantle.Web.ContentManagement/Sitemap.GenerateFileError";
            public const string GenerateFileSuccess = "Mantle.Web.ContentManagement/Sitemap.GenerateFileSuccess";
            public const string Title = "Mantle.Web.ContentManagement/Sitemap.Title";
            public const string XMLSitemap = "Mantle.Web.ContentManagement/Sitemap.XMLSitemap";
        }

        public static class UserProfile
        {
            public static class Newsletter
            {
                public const string SubscribeToNewsletters = "Mantle.Web.ContentManagement/UserProfile.Newsletter.SubscribeToNewsletters";
            }
        }
    }
}