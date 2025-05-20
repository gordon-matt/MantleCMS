using Mantle.Logging.Entities;
using Mantle.Logging.Services;

namespace Mantle.Web.Areas.Admin.Log.Controllers.Api;

public class LogApiController : GenericTenantODataController<LogEntry, int>
{
    public LogApiController(ILogService service)
        : base(service)
    {
    }

    protected override int GetId(LogEntry entity) => entity.Id;

    protected override void SetNewId(LogEntry entity)
    {
    }

    protected override Permission ReadPermission => MantleWebPermissions.LogRead;

    protected override Permission WritePermission => StandardPermissions.FullAccess;

    [HttpPost]
    public virtual async Task<IActionResult> Clear(ODataActionParameters parameters)
    {
        if (!CheckPermission(WritePermission))
        {
            return Unauthorized();
        }

        int tenantId = GetTenantId();
        await Service.DeleteAsync(x => x.TenantId == tenantId);

        return Ok();
    }
}