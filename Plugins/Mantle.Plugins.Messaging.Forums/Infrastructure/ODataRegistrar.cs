using Extenso.AspNetCore.OData;
using Mantle.Plugins.Messaging.Forums.Data.Domain;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

namespace Mantle.Plugins.Messaging.Forums.Infrastructure
{
    public class ODataRegistrar : IODataRegistrar
    {
        #region IODataRegistrar Members

        public void Register(ODataOptions options)
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Forum>("ForumApi");
            builder.EntitySet<ForumGroup>("ForumGroupApi");

            options.AddRouteComponents("odata/Mantle/plugins/forums", builder.GetEdmModel());
        }

        #endregion IODataRegistrar Members
    }
}