using Mantle.Messaging.Data.Entities;
using Microsoft.AspNetCore.OData.Formatter;

namespace Mantle.Web.Messaging.Controllers.Api;

public class MessageTemplateApiController : GenericTenantODataController<MessageTemplate, int>
{
    private readonly Lazy<IEnumerable<IMessageTemplateTokensProvider>> tokensProviders;

    public MessageTemplateApiController(
        IRepository<MessageTemplate> repository,
        Lazy<IEnumerable<IMessageTemplateTokensProvider>> tokensProviders)
        : base(repository)
    {
        this.tokensProviders = tokensProviders;
    }

    protected override int GetId(MessageTemplate entity) => entity.Id;

    protected override void SetNewId(MessageTemplate entity)
    {
    }

    [HttpGet]
    public virtual IEnumerable<string> GetTokens([FromODataUri] string templateName) => !CheckPermission(ReadPermission)
        ? Enumerable.Empty<string>()
        : tokensProviders.Value
            .SelectMany(x => x.GetTokens(templateName))
            .Distinct()
            .OrderBy(x => x)
            .ToList();

    protected override Permission ReadPermission => MessagingPermissions.MessageTemplatesRead;

    protected override Permission WritePermission => MessagingPermissions.MessageTemplatesWrite;
}