﻿using Mantle.Messaging.Data.Entities;
using Mantle.Messaging.Services;

namespace Mantle.Web.Messaging.Controllers.Api;

public class QueuedEmailApiController : GenericTenantODataController<QueuedEmail, Guid>
{
    public QueuedEmailApiController(IQueuedEmailService service)
        : base(service)
    {
    }

    protected override Guid GetId(QueuedEmail entity) => entity.Id;

    protected override void SetNewId(QueuedEmail entity) => entity.Id = Guid.NewGuid();

    protected override Permission ReadPermission => MessagingPermissions.QueuedEmailsRead;

    protected override Permission WritePermission => MessagingPermissions.QueuedEmailsWrite;
}