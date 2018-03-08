using System;
using Mantle.Web.Common.Areas.Admin.Regions.Controllers.Api;
using Mantle.Web.Common.Areas.Admin.Regions.Domain;
using Mantle.Web.Infrastructure;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Mantle.Web.Common.Infrastructure
{
    public class ODataRegistrar : IODataRegistrar
    {
        #region IODataRegistrar Members

        public void Register(IRouteBuilder routes, IServiceProvider services)
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder(services);
            builder.EntitySet<Region>("RegionApi");
            builder.EntitySet<RegionSettings>("RegionSettingsApi");

            RegisterRegionODataActions(builder);
            RegisterRegionSettingsODataActions(builder);

            routes.MapODataServiceRoute("OData_Kore_Common", "odata/mantle/common", builder.GetEdmModel());
        }

        private static void RegisterRegionODataActions(ODataModelBuilder builder)
        {
            var getLocalizedActionFunction = builder.EntityType<Region>().Collection.Function("GetLocalized");
            getLocalizedActionFunction.Parameter<int>("id");
            getLocalizedActionFunction.Parameter<string>("cultureCode");
            getLocalizedActionFunction.ReturnsFromEntitySet<Region>("RegionApi");

            var saveLocalizedAction = builder.EntityType<Region>().Collection.Action("SaveLocalized");
            saveLocalizedAction.Parameter<string>("cultureCode");
            saveLocalizedAction.Parameter<Region>("entity");
            saveLocalizedAction.Returns<IActionResult>();
        }

        private static void RegisterRegionSettingsODataActions(ODataModelBuilder builder)
        {
            var getSettingsAction = builder.EntityType<RegionSettings>().Collection.Action("GetSettings");
            getSettingsAction.Parameter<string>("settingsId");
            getSettingsAction.Parameter<int>("regionId");
            getSettingsAction.Returns<EdmRegionSettings>();

            var saveSettingsAction = builder.EntityType<RegionSettings>().Collection.Action("SaveSettings");
            saveSettingsAction.Parameter<string>("settingsId");
            saveSettingsAction.Parameter<int>("regionId");
            saveSettingsAction.Parameter<string>("fields");
            saveSettingsAction.Returns<IActionResult>();
        }

        #endregion IODataRegistrar Members
    }
}