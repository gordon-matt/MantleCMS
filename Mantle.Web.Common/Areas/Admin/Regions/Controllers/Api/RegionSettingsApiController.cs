using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Mantle.Infrastructure;
using Mantle.Web.Common.Areas.Admin.Regions.Domain;
using Mantle.Web.Common.Areas.Admin.Regions.Services;
using Mantle.Web.Mvc;
using Mantle.Web.Mvc.KendoUI;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Common.Areas.Admin.Regions.Controllers.Api
{
    [Area(Constants.Areas.Regions)]
    [Route("api/region-settings")]
    public class RegionSettingsApiController : MantleController
    {
        private readonly IEnumerable<IRegionSettings> regionSettings;
        private readonly Lazy<IRegionSettingsService> regionSettingsService;

        public RegionSettingsApiController(
            IEnumerable<IRegionSettings> regionSettings,
            Lazy<IRegionSettingsService> regionSettingsService)
        {
            this.regionSettings = regionSettings;
            this.regionSettingsService = regionSettingsService;
        }

        [HttpPost]
        [Route("get")]
        public virtual async Task<IActionResult> Get([FromBody]KendoGridMvcRequest request)
        {
            if (!CheckPermission(Permissions.RegionsRead))
            {
                return Unauthorized();
            }

            var query = regionSettings
                .Select(x => new EdmRegionSettings
                {
                    Id = x.Name.ToSlugUrl(),
                    Name = x.Name
                })
                .AsQueryable();

            var grid = new CustomKendoGridEx<EdmRegionSettings>(request, query);
            return Json(grid);
        }

        [HttpPost]
        [Route("Default.GetSettings")]
        public virtual async Task<EdmRegionSettings> GetSettings([FromBody]dynamic data)
        {
            if (!CheckPermission(Permissions.RegionsRead))
            {
                return null;
            }

            string settingsId = data.settingsId;
            int regionId = data.regionId;

            var dictionary = regionSettings.ToDictionary(k => k.Name.ToSlugUrl(), v => v);

            if (!dictionary.ContainsKey(settingsId))
            {
                return null;
            }

            var settings = dictionary[settingsId];

            var dataEntity = await regionSettingsService.Value.FindOneAsync(x =>
                x.RegionId == regionId &&
                x.SettingsId == settingsId);

            if (dataEntity != null)
            {
                return new EdmRegionSettings
                {
                    Id = settingsId,
                    Name = settings.Name,
                    Fields = dataEntity.Fields
                };
            }

            return new EdmRegionSettings
            {
                Id = settingsId,
                Name = settings.Name,
                Fields = null
            };
        }

        [HttpPost]
        [Route("Default.SaveSettings")]
        public virtual async Task<IActionResult> SaveSettings([FromBody]dynamic data)
        {
            if (!CheckPermission(Permissions.RegionsWrite))
            {
                return Unauthorized();
            }

            string settingsId = data.settingsId;
            int regionId = data.regionId;
            string fields = data.fields;

            if (string.IsNullOrEmpty(settingsId))
            {
                return this.BadRequest("SettingsId has not been provided");
            }
            if (regionId == 0)
            {
                return this.BadRequest("RegionId has not been provided.");
            }

            var allSettingsIds = regionSettings.Select(x => x.Name.ToSlugUrl());

            if (!allSettingsIds.Contains(settingsId))
            {
                return this.BadRequest(string.Format("SettingsId, '{0}' is not recognized.", settingsId));
            }

            var dataEntity = await regionSettingsService.Value.FindOneAsync(x =>
                x.RegionId == regionId &&
                x.SettingsId == settingsId);

            if (dataEntity == null)
            {
                dataEntity = new RegionSettings
                {
                    SettingsId = settingsId,
                    RegionId = regionId,
                    Fields = fields
                };
                await regionSettingsService.Value.InsertAsync(dataEntity);
                return Ok();
                //TODO (currently throws error because of missing entity set on odata, basically we need to create
                //      the separate Api Controller for it. A good idea might be to make this controller do that and make the
                //      current Get() method an OData action instead... and maybe call it GetSettingsTypes())
                //return Created(dataEntity);
            }
            else
            {
                dataEntity.Fields = fields;
                await regionSettingsService.Value.UpdateAsync(dataEntity);
                return Ok();
                //TODO (currently throws error because of missing entity set on odata, basically we need to create
                //      the separate Api Controller for it. A good idea might be to make this controller do that and make the
                //      current Get() method an OData action instead... and maybe call it GetSettingsTypes())
                //return Updated(dataEntity);
            }
        }

        protected static bool CheckPermission(Permission permission)
        {
            var authorizationService = EngineContext.Current.Resolve<IAuthorizationService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            return authorizationService.TryCheckAccess(permission, workContext.CurrentUser);
        }
    }

    public class EdmRegionSettings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Fields { get; set; }
    }
}