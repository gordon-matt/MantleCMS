using Mantle.Data;
using Mantle.Messaging.Data.Domain;
using Mantle.Web.OData;
using Mantle.Web.Security.Membership.Permissions;

namespace Mantle.Web.Messaging.Controllers.Api
{
    public class MessageTemplateVersionVersionApiController : GenericODataController<MessageTemplateVersion, int>
    {
        public MessageTemplateVersionVersionApiController(
            IRepository<MessageTemplateVersion> repository)
            : base(repository)
        {
        }

        protected override int GetId(MessageTemplateVersion entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(MessageTemplateVersion entity)
        {
        }

        protected override Permission ReadPermission
        {
            get { return MessagingPermissions.MessageTemplatesRead; }
        }

        protected override Permission WritePermission
        {
            get { return MessagingPermissions.MessageTemplatesWrite; }
        }
    }
}