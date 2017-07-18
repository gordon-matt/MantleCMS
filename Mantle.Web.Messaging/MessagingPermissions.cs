using System.Collections.Generic;
using Mantle.Web.Security.Membership.Permissions;

namespace Mantle.Web.Messaging
{
    public class MessagingPermissions : IPermissionProvider
    {
        public static readonly Permission MessageTemplatesRead = new Permission { Name = "MessageTemplates_Read", Category = "Messaging", Description = "Mantle - Message Templates: Read" };
        public static readonly Permission MessageTemplatesWrite = new Permission { Name = "MessageTemplates_Write", Category = "Messaging", Description = "Mantle - Message Templates: Write" };
        public static readonly Permission QueuedEmailsRead = new Permission { Name = "QueuedEmails_Read", Category = "Messaging", Description = "Mantle - Queued Emails: Read" };
        public static readonly Permission QueuedEmailsWrite = new Permission { Name = "QueuedEmails_Write", Category = "Messaging", Description = "Mantle - Queued Emails: Write" };

        public IEnumerable<Permission> GetPermissions()
        {
            yield return MessageTemplatesRead;
            yield return MessageTemplatesWrite;

            yield return QueuedEmailsRead;
            yield return QueuedEmailsWrite;
        }
    }
}