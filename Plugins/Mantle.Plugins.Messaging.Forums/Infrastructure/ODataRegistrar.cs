using System;
using Mantle.Plugins.Messaging.Forums.Data.Domain;
using Mantle.Web.Infrastructure;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Routing;

namespace Mantle.Plugins.Messaging.Forums.Infrastructure
{
    public class ODataRegistrar : IODataRegistrar
    {
        #region IODataRegistrar Members

        public void Register(IRouteBuilder routes, IServiceProvider services)
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder(services);
            builder.EntitySet<Forum>("ForumApi");
            builder.EntitySet<ForumGroup>("ForumGroupApi");

            routes.MapODataServiceRoute("OData_Mantle_Plugin_Forums", "odata/Mantle/plugins/forums", builder.GetEdmModel());
        }

        #endregion IODataRegistrar Members
    }
}