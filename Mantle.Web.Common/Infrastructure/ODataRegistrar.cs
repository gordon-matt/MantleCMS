using Extenso.AspNetCore.OData;
using Mantle.Web.Common.Areas.Admin.Regions.Controllers.Api;
using Mantle.Web.Common.Areas.Admin.Regions.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

namespace Mantle.Web.Common.Infrastructure
{
    public class ODataRegistrar : IODataRegistrar
    {
        #region IODataRegistrar Members

        public void Register(ODataOptions options)
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Region>("RegionApi");
            builder.EntitySet<RegionSettings>("RegionSettingsApi");

            RegisterRegionODataActions(builder);
            RegisterRegionSettingsODataActions(builder);

            options.AddRouteComponents("odata/mantle/common", builder.GetEdmModel());
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
            var getSettingsFunction = builder.EntityType<RegionSettings>().Collection.Function("GetSettings");
            getSettingsFunction.Parameter<string>("settingsId");
            getSettingsFunction.Parameter<int>("regionId");
            getSettingsFunction.Returns<EdmRegionSettings>();

            var saveSettingsAction = builder.EntityType<RegionSettings>().Collection.Action("SaveSettings");
            saveSettingsAction.Parameter<string>("settingsId");
            saveSettingsAction.Parameter<int>("regionId");
            saveSettingsAction.Parameter<string>("fields");
            saveSettingsAction.Returns<IActionResult>();
        }

        #endregion IODataRegistrar Members
    }
}