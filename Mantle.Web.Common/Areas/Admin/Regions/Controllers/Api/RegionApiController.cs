using System;
using System.Linq;
using System.Threading.Tasks;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Mantle.Localization.Domain;
using Mantle.Localization.Services;
using Mantle.Web.Common.Areas.Admin.Regions.Domain;
using Mantle.Web.Common.Areas.Admin.Regions.Services;
using Mantle.Web.Mvc;
using Mantle.Web.Mvc.KendoUI;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Common.Areas.Admin.Regions.Controllers.Api
{
    [Area(Constants.Areas.Regions)]
    [Route("api/regions")]
    public class RegionApiController : MantleGenericTenantDataController<Region, int>
    {
        private readonly Lazy<ILocalizablePropertyService> localizablePropertyService;

        public RegionApiController(
            IRegionService service,
            Lazy<ILocalizablePropertyService> localizablePropertyService)
            : base(service)
        {
            this.localizablePropertyService = localizablePropertyService;
        }

        [HttpPost]
        [Route("filter-by-region-type")]
        public virtual async Task<IActionResult> FilterByRegionType([FromBody]KendoGridMvcRequest request, string regionType = null, int? parentId = null)
        {
            if (!CheckPermission(ReadPermission))
            {
                return Unauthorized();
            }

            using (var connection = Service.OpenConnection())
            {
                var query = connection.Query();
                query = ApplyMandatoryFilter(query);

                if (!string.IsNullOrEmpty(regionType))
                {
                    var type = Mantle.EnumExtensions.Parse<RegionType>(regionType);
                    query = query.Where(x => x.RegionType == type);
                }

                if (parentId.HasValue)
                {
                    query = query.Where(x => x.ParentId == parentId);
                }

                var grid = new CustomKendoGridEx<Region>(request, query);
                return Json(grid);
            }
        }

        protected override int GetId(Region entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(Region entity)
        {
        }

        [HttpGet]
        [Route("Default.GetLocalized")]
        public async Task<IActionResult> GetLocalized([FromBody]dynamic data)
        {
            if (!CheckPermission(ReadPermission))
            {
                return Unauthorized();
            }

            int id = data.id;
            string cultureCode = data.cultureCode;

            if (id == 0)
            {
                return BadRequest();
            }

            var entity = await Service.FindOneAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            string entityType = typeof(Region).FullName;
            string entityId = entity.Id.ToString();

            var localizedRecord = await localizablePropertyService.Value.FindOneAsync(cultureCode, entityType, entityId, "Name");
            if (localizedRecord != null)
            {
                entity.Name = localizedRecord.Value;
            }

            return Ok(entity);
        }

        [HttpPost]
        [Route("Default.SaveLocalized")]
        public async Task<IActionResult> SaveLocalized([FromBody]dynamic data)
        {
            if (!CheckPermission(WritePermission))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string cultureCode = data.cultureCode;
            Region entity = data.entity;

            if (entity.Id == 0)
            {
                return BadRequest();
            }
            string entityType = typeof(Region).FullName;
            string entityId = entity.Id.ToString();

            var localizedRecord = await localizablePropertyService.Value.FindOneAsync(cultureCode, entityType, entityId, "Name");
            if (localizedRecord == null)
            {
                localizedRecord = new LocalizableProperty
                {
                    CultureCode = cultureCode,
                    EntityType = entityType,
                    EntityId = entityId,
                    Property = "Name",
                    Value = entity.Name
                };
                await localizablePropertyService.Value.InsertAsync(localizedRecord);
                return Ok();
            }
            else
            {
                localizedRecord.Value = entity.Name;
                await localizablePropertyService.Value.UpdateAsync(localizedRecord);
                return Ok();
            }
        }

        protected override Permission ReadPermission
        {
            get { return Permissions.RegionsRead; }
        }

        protected override Permission WritePermission
        {
            get { return Permissions.RegionsWrite; }
        }
    }
}