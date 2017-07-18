using System;
using System.Threading.Tasks;
using Mantle.Data;
using Mantle.Tasks.Domain;
using Mantle.Web.Mvc;
using Mantle.Web.Mvc.KendoUI;
using Mantle.Web.Security.Membership.Permissions;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MantleTask = Mantle.Tasks.Task;

namespace Mantle.Web.Areas.Admin.ScheduledTasks.Controllers.Api
{
    [Area(MantleWebConstants.Areas.ScheduledTasks)]
    [Route("api/scheduled-tasks")]
    public class ScheduledTaskApiController : MantleGenericDataController<ScheduledTask, int>
    {
        public ScheduledTaskApiController(IRepository<ScheduledTask> repository)
            : base(repository)
        {
        }

        [HttpPost]
        [Route("get")]
        public override async Task<IActionResult> Get([FromBody]KendoGridMvcRequest request)
        {
            return await base.Get(request);
        }

        [HttpGet]
        [Route("{key}")]
        public override async Task<IActionResult> Get(int key)
        {
            return await base.Get(key);
        }

        [HttpPut]
        [Route("{key}")]
        public override async Task<IActionResult> Put(int key, [FromBody]ScheduledTask entity)
        {
            var existingEntity = await Service.FindOneAsync(key);
            existingEntity.Seconds = entity.Seconds;
            existingEntity.Enabled = entity.Enabled;
            existingEntity.StopOnError = entity.StopOnError;
            return await base.Put(key, existingEntity);
        }

        [HttpPost]
        [Route("")]
        public override async Task<IActionResult> Post([FromBody]ScheduledTask entity)
        {
            return await base.Post(entity);
        }

        [HttpDelete]
        [Route("{key}")]
        public override async Task<IActionResult> Delete(int key)
        {
            return await base.Delete(key);
        }

        [HttpPost]
        [Route("Default.RunNow")]
        public async Task<IActionResult> RunNow([FromBody]dynamic data)
        {
            if (!CheckPermission(WritePermission))
            {
                return Unauthorized();
            }

            int taskId = data.taskId;

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

        protected override int GetId(ScheduledTask entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(ScheduledTask entity)
        {
            // Do nothing (int is auto incremented)
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
}