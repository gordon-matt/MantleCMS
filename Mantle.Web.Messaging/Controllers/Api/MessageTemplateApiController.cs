using Mantle.Messaging.Data.Entities;

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

    protected override int GetId(MessageTemplate entity)
    {
        return entity.Id;
    }

    protected override void SetNewId(MessageTemplate entity)
    {
    }

    [HttpGet]
    public virtual IEnumerable<string> GetTokens([FromODataUri] string templateName)
    {
        if (!CheckPermission(ReadPermission))
        {
            return Enumerable.Empty<string>();
        }

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