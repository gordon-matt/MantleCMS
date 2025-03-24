using Mantle.Localization.Entities;
using Mantle.Web.Common.Areas.Admin.Regions.Entities;
using Mantle.Web.Common.Areas.Admin.Regions.Services;

namespace Mantle.Web.Common.Areas.Admin.Regions.Controllers.Api;

public class RegionApiController : GenericTenantODataController<Region, int>
{
    private readonly Lazy<ILocalizablePropertyService> localizablePropertyService;

    public RegionApiController(
        IRegionService service,
        Lazy<ILocalizablePropertyService> localizablePropertyService)
        : base(service)
    {
        this.localizablePropertyService = localizablePropertyService;
    }

    protected override int GetId(Region entity) => entity.Id;

    protected override void SetNewId(Region entity)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetLocalized([FromODataUri] int id, [FromODataUri] string cultureCode)
    {
        if (!CheckPermission(ReadPermission))
        {
            return Unauthorized();
        }

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

        var localizedRecord = localizablePropertyService.Value.FindOne(cultureCode, entityType, entityId, "Name");
        if (localizedRecord != null)
        {
            entity.Name = localizedRecord.Value;
        }

        return Ok(entity);
    }

    [HttpPost]
    public async Task<IActionResult> SaveLocalized([FromBody] ODataActionParameters parameters)
    {
        if (!CheckPermission(WritePermission))
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string cultureCode = (string)parameters["cultureCode"];
        var entity = (Region)parameters["entity"];

        if (entity.Id == 0)
        {
            return BadRequest();
        }
        string entityType = typeof(Region).FullName;
        string entityId = entity.Id.ToString();

        var localizedRecord = localizablePropertyService.Value.FindOne(cultureCode, entityType, entityId, "Name");
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

    protected override Permission ReadPermission => Permissions.RegionsRead;

    protected override Permission WritePermission => Permissions.RegionsWrite;
}