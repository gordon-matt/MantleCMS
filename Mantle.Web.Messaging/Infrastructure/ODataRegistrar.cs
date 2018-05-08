using System;
using Mantle.Messaging.Data.Domain;
using Mantle.Web.Infrastructure;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Routing;

namespace Mantle.Web.Messaging.Infrastructure
{
    public class ODataRegistrar : IODataRegistrar
    {
        public void Register(IRouteBuilder routes, IServiceProvider services)
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder(services);

            builder.EntitySet<MessageTemplate>("MessageTemplateApi");
            builder.EntitySet<MessageTemplateVersion>("MessageTemplateVersionApi");
            builder.EntitySet<QueuedEmail>("QueuedEmailApi");

            RegisterMessageTemplateODataActions(builder);
            RegisterMessageTemplateVersionODataActions(builder);

            routes.MapODataServiceRoute("OData_Mantle_Web_Messaging", "odata/mantle/web/messaging", builder.GetEdmModel());
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
            getCurrentVersionFunction.Returns<MessageTemplateVersion>();
        }
    }
}