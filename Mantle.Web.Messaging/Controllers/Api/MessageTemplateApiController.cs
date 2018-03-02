using System;
using System.Collections.Generic;
using System.Linq;
using Mantle.Data;
using Mantle.Messaging.Data.Domain;
using Mantle.Web.OData;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Messaging.Controllers.Api
{
    public class MessageTemplateApiController : GenericTenantODataController<MessageTemplate, Guid>
    {
        private readonly Lazy<IEnumerable<IMessageTemplateTokensProvider>> tokensProviders;

        public MessageTemplateApiController(
            IRepository<MessageTemplate> repository,
            Lazy<IEnumerable<IMessageTemplateTokensProvider>> tokensProviders)
            : base(repository)
        {
            this.tokensProviders = tokensProviders;
        }

        protected override Guid GetId(MessageTemplate entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(MessageTemplate entity)
        {
            entity.Id = Guid.NewGuid();
        }

        [HttpPost]
        public virtual IEnumerable<string> GetTokens(ODataActionParameters parameters)
        {
            if (!CheckPermission(ReadPermission))
            {
                return Enumerable.Empty<string>();
            }

            string templateName = (string)parameters["templateName"];

            return tokensProviders.Value
                .SelectMany(x => x.GetTokens(templateName))
                .Distinct()
                .OrderBy(x => x)
                .ToList();
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