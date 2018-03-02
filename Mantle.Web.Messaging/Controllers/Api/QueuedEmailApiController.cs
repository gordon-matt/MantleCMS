using System;
using Mantle.Messaging.Data.Domain;
using Mantle.Messaging.Services;
using Mantle.Web.OData;
using Mantle.Web.Security.Membership.Permissions;

namespace Mantle.Web.Messaging.Controllers.Api
{
    public class QueuedEmailApiController : GenericTenantODataController<QueuedEmail, Guid>
    {
        public QueuedEmailApiController(IQueuedEmailService service)
            : base(service)
        {
        }

        protected override Guid GetId(QueuedEmail entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(QueuedEmail entity)
        {
            entity.Id = Guid.NewGuid();
        }

        protected override Permission ReadPermission
        {
            get { return MessagingPermissions.QueuedEmailsRead; }
        }

        protected override Permission WritePermission
        {
            get { return MessagingPermissions.QueuedEmailsWrite; }
        }
    }
}