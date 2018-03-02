﻿using System;
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

            // Messaging
            builder.EntitySet<MessageTemplate>("MessageTemplateApi");
            builder.EntitySet<QueuedEmail>("QueuedEmailApi");

            RegisterMessageTemplateODataActions(builder);

            routes.MapODataServiceRoute("OData_Mantle_Web_Messaging", "odata/mantle/web/messaging", builder.GetEdmModel());
        }

        private static void RegisterMessageTemplateODataActions(ODataModelBuilder builder)
        {
            var getTokensAction = builder.EntityType<MessageTemplate>().Collection.Action("GetTokens");
            getTokensAction.Parameter<string>("templateName");
            getTokensAction.ReturnsCollection<string>();
        }
    }
}