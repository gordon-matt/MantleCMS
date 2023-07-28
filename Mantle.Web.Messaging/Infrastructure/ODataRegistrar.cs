using Extenso.AspNetCore.OData;
using Mantle.Messaging.Data.Domain;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

namespace Mantle.Web.Messaging.Infrastructure;

public class ODataRegistrar : IODataRegistrar
{
    public void Register(ODataOptions options)
    {
        ODataModelBuilder builder = new ODataConventionModelBuilder();

        builder.EntitySet<MessageTemplate>("MessageTemplateApi");
        builder.EntitySet<MessageTemplateVersion>("MessageTemplateVersionApi");
        builder.EntitySet<QueuedEmail>("QueuedEmailApi");

        RegisterMessageTemplateODataActions(builder);
        RegisterMessageTemplateVersionODataActions(builder);

        options.AddRouteComponents("odata/mantle/web/messaging", builder.GetEdmModel());
    }

    private static void RegisterMessageTemplateODataActions(ODataModelBuilder builder)
    {
        var getTokensFunction = builder.EntityType<MessageTemplate>().Collection.Function("GetTokens");
        getTokensFunction.Parameter<string>("templateName");
        getTokensFunction.ReturnsCollection<string>();
    }

    private static void RegisterMessageTemplateVersionODataActions(ODataModelBuilder builder)
    {
        var getCurrentVersionFunction = builder.EntityType<MessageTemplateVersion>().Collection.Function("GetCurrentVersion");
        getCurrentVersionFunction.Parameter<int>("templateId");
        getCurrentVersionFunction.Parameter<string>("cultureCode");
        getCurrentVersionFunction.ReturnsFromEntitySet<MessageTemplateVersion>("MessageTemplateVersionApi");
    }
}