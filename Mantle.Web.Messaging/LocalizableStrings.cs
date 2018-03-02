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
            public const string Body = "Mantle.Web.Messaging/MessageTemplate.Body";
            public const string Name = "Mantle.Web.Messaging/MessageTemplate.Name";
            public const string Subject = "Mantle.Web.Messaging/MessageTemplate.Subject";
            public const string Tokens = "Mantle.Web.Messaging/MessageTemplate.Tokens";
        }

        public static class QueuedEmail
        {
            public const string CreatedOnUtc = "Mantle.Web.Messaging/QueuedEmail.CreatedOnUtc";
            public const string SentOnUtc = "Mantle.Web.Messaging/QueuedEmail.SentOnUtc";
            public const string SentTries = "Mantle.Web.Messaging/QueuedEmail.SentTries";
            public const string Subject = "Mantle.Web.Messaging/QueuedEmail.Subject";
            public const string ToAddress = "Mantle.Web.Messaging/QueuedEmail.ToAddress";
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