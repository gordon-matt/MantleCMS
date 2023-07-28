using MantleTask = Mantle.Tasks.Task;

namespace Mantle.Web.Areas.Admin.ScheduledTasks.Controllers.Api;

public class ScheduledTaskApiController : GenericODataController<ScheduledTask, int>
{
    public ScheduledTaskApiController(IRepository<ScheduledTask> repository)
        : base(repository)
    {
    }

    protected override int GetId(ScheduledTask entity)
    {
        return entity.Id;
    }

    protected override void SetNewId(ScheduledTask entity)
    {
        // Do nothing (int is auto incremented)
    }

    public override async Task<IActionResult> Put(int key, ScheduledTask entity)
    {
        var existingEntity = await Service.FindOneAsync(key);
        existingEntity.Seconds = entity.Seconds;
        existingEntity.Enabled = entity.Enabled;
        existingEntity.StopOnError = entity.StopOnError;
        return await base.Put(key, existingEntity);
    }

    [HttpPost]
    public async Task<IActionResult> RunNow([FromBody] ODataActionParameters parameters)
    {
        if (!CheckPermission(WritePermission))
        {
            return Unauthorized();
        }

        int taskId = (int)parameters["taskId"];

        var scheduleTask = await Service.FindOneAsync(taskId);
        if (scheduleTask == null)
        {
            return NotFound();
        }

        var task = new MantleTask(scheduleTask);
        //ensure that the task is enabled
        task.Enabled = true;

        try
        {
            task.Execute(true);
        }
        catch (Exception x)
        {
            Logger.LogError(new EventId(), x, x.Message);
            return StatusCode(500, x);
        }

        return Ok();
    }

    protected override Permission ReadPermission
    {
        get { return MantleWebPermissions.ScheduledTasksRead; }
    }

    protected override Permission WritePermission
    {
        get { return MantleWebPermissions.ScheduledTasksWrite; }
    }
}