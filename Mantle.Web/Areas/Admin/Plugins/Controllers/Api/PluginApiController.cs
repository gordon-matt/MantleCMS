using System;
using System.Linq;
using System.Threading.Tasks;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Mantle.Collections;
using Mantle.Web.Areas.Admin.Plugins.Models;
using Mantle.Web.Mvc;
using Mantle.Web.Mvc.KendoUI;
using Mantle.Web.Plugins;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Mantle.Web.Areas.Admin.Plugins.Controllers.Api
{
    [Route("api/plugins")]
    public class PluginApiController : MantleController
    {
        private readonly IPluginFinder pluginFinder;

        public PluginApiController(IPluginFinder pluginFinder)
        {
            this.pluginFinder = pluginFinder;
        }

        [HttpPost]
        [Route("get")]
        public virtual async Task<IActionResult> Get([FromBody]KendoGridMvcRequest request)
        {
            if (!CheckPermission(StandardPermissions.FullAccess))
            {
                return Unauthorized();
            }

            var query = pluginFinder.GetPluginDescriptors(false).Select(x => (EdmPluginDescriptor)x).AsQueryable();
            var grid = new CustomKendoGridEx<EdmPluginDescriptor>(request, query);
            return Json(grid);
        }

        [HttpGet]
        [Route("{key}")]
        public virtual async Task<IActionResult> Get(string key)
        {
            if (!CheckPermission(StandardPermissions.FullAccess))
            {
                return Unauthorized();
            }

            string systemName = key.Replace('-', '.');
            var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName(systemName, false);
            EdmPluginDescriptor entity = pluginDescriptor;

            return Json(JObject.FromObject(entity));
        }

        [HttpPut]
        [Route("{key}")]
        public virtual async Task<IActionResult> Put(string key, [FromBody]EdmPluginDescriptor entity)
        {
            if (!CheckPermission(StandardPermissions.FullAccess))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string systemName = key.Replace('-', '.');
                var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName(systemName, false);

                if (pluginDescriptor == null)
                {
                    return NotFound();
                }

                pluginDescriptor.FriendlyName = entity.FriendlyName;
                pluginDescriptor.DisplayOrder = entity.DisplayOrder;
                pluginDescriptor.LimitedToTenants.Clear();
                if (!entity.LimitedToTenants.IsNullOrEmpty())
                {
                    pluginDescriptor.LimitedToTenants = entity.LimitedToTenants.ToList();
                }
                PluginFileParser.SavePluginDescriptionFile(pluginDescriptor);
            }
            catch (Exception x)
            {
                Logger.LogError(new EventId(), x, x.GetBaseException().Message);
            }

            return Ok(entity);
        }
    }
}