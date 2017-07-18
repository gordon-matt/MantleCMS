namespace Mantle.Web.Messaging
{
    public static class LocalizableStrings
    {
        public const string GetTokensError = "Mantle.Web.Messaging/GetTokensError";
        public const string MessageTemplates = "Mantle.Web.Messaging/MessageTemplates";
        public const string Messaging = "Mantle.Web.Messaging/Messaging";
        public const string QueuedEmails = "Mantle.Web.Messaging/QueuedEmails";

        public static class MessageTemplate
        {
            public const string Body = "Mantle.Web.Messaging/MessageTemplateModel.Body";
            public const string Name = "Mantle.Web.Messaging/MessageTemplateModel.Name";
            public const string Subject = "Mantle.Web.Messaging/MessageTemplateModel.Subject";
            public const string Tokens = "Mantle.Web.Messaging/MessageTemplateModel.Tokens";
        }

        public static class QueuedEmail
        {
            public const string CreatedOnUtc = "Mantle.Web.Messaging/QueuedEmailModel.CreatedOnUtc";
            public const string SentOnUtc = "Mantle.Web.Messaging/QueuedEmailModel.SentOnUtc";
            public const string SentTries = "Mantle.Web.Messaging/QueuedEmailModel.SentTries";
            public const string Subject = "Mantle.Web.Messaging/QueuedEmailModel.Subject";
            public const string ToAddress = "Mantle.Web.Messaging/QueuedEmailModel.ToAddress";
        }

        public static class Settings
        {
            public static class Smtp
            {
                public const string DisplayName = "Mantle.Web.Messaging/Settings.Smtp.DisplayName";
                public const string EnableSsl = "Mantle.Web.Messaging/Settings.Smtp.EnableSsl";
                public const string Host = "Mantle.Web.Messaging/Settings.Smtp.Host";
                public const string MaxTries = "Mantle.Web.Messaging/Settings.Smtp.MaxTries";
                public const string MessagesPerBatch = "Mantle.Web.Messaging/Settings.Smtp.MessagesPerBatch";
                public const string Password = "Mantle.Web.Messaging/Settings.Smtp.Password";
                public const string Port = "Mantle.Web.Messaging/Settings.Smtp.Port";
                public const string UseDefaultCredentials = "Mantle.Web.Messaging/Settings.Smtp.UseDefaultCredentials";
                public const string Username = "Mantle.Web.Messaging/Settings.Smtp.Username";
            }
        }
    }
}