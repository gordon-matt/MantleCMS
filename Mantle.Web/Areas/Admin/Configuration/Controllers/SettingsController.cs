using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mantle.Infrastructure;
using Mantle.Web.Configuration;
using Mantle.Web.Mvc;
using Mantle.Web.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Areas.Admin.Configuration.Controllers
{
    [Authorize]
    [Area(MantleWebConstants.Areas.Configuration)]
    [Route("admin/configuration/settings")]
    public class SettingsController : MantleController
    {
        private readonly Lazy<IEnumerable<ISettings>> settings;

        public SettingsController(Lazy<IEnumerable<ISettings>> settings)
            : base()
        {
            this.settings = settings;
        }
        
        [Route("")]
        public IActionResult Index()
        {
            if (!CheckPermission(MantleWebPermissions.SettingsRead))
            {
                return Unauthorized();
            }

            WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.General.Configuration].Value);
            WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.General.Settings].Value);

            ViewBag.Title = T[MantleWebLocalizableStrings.General.Configuration].Value;
            ViewBag.SubTitle = T[MantleWebLocalizableStrings.General.Settings].Value;

            return PartialView();
        }

        [Route("get-view-data")]
        public JsonResult GetViewData()
        {
            return Json(new
            {
                gridPageSize = SiteSettings.Value.DefaultGridPageSize,
                translations = new
                {
                    edit = T[MantleWebLocalizableStrings.General.Edit].Value,
                    getRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                    updateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                    updateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                    columns = new
                    {
                        name = T[MantleWebLocalizableStrings.Settings.Model.Name].Value,
                    }
                }
            });
        }

        [Route("get-editor-ui/{type}")]
        public async Task<IActionResult> GetEditorUI(string type)
        {
            var model = settings.Value.FirstOrDefault(x => x.GetType().FullName == type.Replace('-', '.'));

            if (model == null)
            {
                return NotFound();
            }

            var razorViewRenderService = EngineContext.Current.Resolve<IRazorViewRenderService>();
            string content = await razorViewRenderService.RenderToStringAsync(model.EditorTemplatePath, model);
            return Json(new { Content = content });
        }
    }
}