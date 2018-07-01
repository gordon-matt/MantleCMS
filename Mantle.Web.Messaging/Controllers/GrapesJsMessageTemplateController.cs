using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Extenso;
using Extenso.Collections;
using Mantle.Messaging.Data.Domain;
using Mantle.Messaging.Services;
using Mantle.Web.Messaging.Models;
using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Messaging.Controllers
{
    [Authorize]
    [Area(MantleWebMessagingConstants.RouteArea)]
    [Route("admin/messaging/grapes-js-templates")]
    public class GrapesJsMessageTemplateController : MantleController
    {
        private readonly Lazy<IMessageTemplateService> messageTemplateService;
        private readonly Lazy<IMessageTemplateVersionService> messageTemplateVersionService;
        private readonly IHostingEnvironment hostingEnvironment;

        public GrapesJsMessageTemplateController(
            Lazy<IMessageTemplateService> messageTemplateService,
            Lazy<IMessageTemplateVersionService> messageTemplateVersionService,
            IHostingEnvironment hostingEnvironment)
        {
            this.messageTemplateService = messageTemplateService;
            this.messageTemplateVersionService = messageTemplateVersionService;
            this.hostingEnvironment = hostingEnvironment;
        }

        [Route("edit/{id}/{cultureCode?}")]
        public async Task<IActionResult> Edit(int id, string cultureCode = null)
        {
            ViewBag.Title = "Email Template Editor";

            var model = await messageTemplateVersionService.Value.FindOneAsync(x =>
                x.MessageTemplateId == id &&
                x.CultureCode == cultureCode);

            if (model == null)
            {
                var template = await messageTemplateService.Value.FindOneAsync(id);

                var utcNow = DateTime.UtcNow;

                model = new MessageTemplateVersion
                {
                    MessageTemplateId = id,
                    CultureCode = cultureCode,
                    Subject = template.Name,
                    DateCreatedUtc = utcNow,
                    DateModifiedUtc = utcNow
                };

                await messageTemplateVersionService.Value.InsertAsync(model);
            }

            // TODO: This is for demo only, we need some way to get tokens for the template and pass them in here...
            //  Then we also need some code for replacing those tokens with real values (in some Campaign Controller or other) and send the email..
            ViewBag.FieldTokens = new Dictionary<string, string>
            {
                { "Title", "Title" },
                { "FirstName", "First Name" },
                { "LastName", "Last Name" },
            };

            return View("Mantle.Web.Messaging.Views.GrapesJsMessageTemplate.Edit", model);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await messageTemplateVersionService.Value.DeleteAsync(x => x.MessageTemplateId == id);
            await messageTemplateService.Value.DeleteAsync(x => x.Id == id);
            return Ok();
        }

        [HttpPost]
        [Route("create/{name}")]
        public async Task<IActionResult> Create(string name)
        {
            var utcNow = DateTime.UtcNow;

            var template = new MessageTemplate
            {
                Name = name,
            };

            await messageTemplateService.Value.InsertAsync(template);

            var version = new MessageTemplateVersion
            {
                MessageTemplateId = template.Id,
                DateCreatedUtc = utcNow,
                DateModifiedUtc = utcNow
            };

            await messageTemplateVersionService.Value.InsertAsync(version);

            return Json(new { status = true, id = template.Id });
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(MessageTemplate model)
        {
            var entity = await messageTemplateService.Value.FindOneAsync(model.Id);
            entity.Name = model.Name;

            await messageTemplateService.Value.UpdateAsync(entity);
            return Json(new { status = true, redirectUrl = Url.Action("Index") });
        }

        #region Storage Manager

        [Route("load/{id}/{cultureCode?}")]
        public async Task<IActionResult> Load(int id, string cultureCode)
        {
            var model = await messageTemplateVersionService.Value.FindOneAsync(x =>
                x.MessageTemplateId == id &&
                x.CultureCode == cultureCode);

            if (!string.IsNullOrEmpty(model.Data))
            {
                var data = model.Data.JsonDeserialize<GrapesJsStorageData>();
                return Json(data);
            }
            else
            {
                return Json(new GrapesJsStorageData());
            }
        }

        [HttpPost]
        [Route("save/{id}/{cultureCode?}")]
        public async Task<IActionResult> Save(int id, string cultureCode, [FromBody] GrapesJsStorageData data)
        {
            var entity = await messageTemplateVersionService.Value.FindOneAsync(x =>
                x.MessageTemplateId == id &&
                x.CultureCode == cultureCode);

            entity.Data = data.JsonSerialize();

            await messageTemplateVersionService.Value.UpdateAsync(entity);

            return Json(new { });
        }

        #endregion Storage Manager

        #region Asset Manager

        [HttpPost]
        [Route("assets/upload")]
        public async Task<IActionResult> UploadAsset(Guid emailTemplateId, string cultureCode)
        {
            var files = Request.Form.Files;

            if (files.IsNullOrEmpty())
            {
                return BadRequest();
            }

            var newAssets = new List<GrapesJsAsset>();
            foreach (var file in files)
            {
                string path = Path.Combine(
                    hostingEnvironment.WebRootPath,
                    "Media\\Uploads\\",
                    WorkContext.CurrentTenant.Id.ToString(),
                    "EmailTemplates\\",
                    emailTemplateId.ToString(),
                    file.FileName);

                newAssets.Add(new GrapesJsAsset
                {
                    Type = "image",
                    Source = path.RightOf("\\Media\\").Replace("\\", "/").Prepend("/")
                });

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            return Json(new { data = newAssets });
        }

        [HttpPost]
        [Route("assets/delete")]
        public IActionResult DeleteAsset([FromBody] GrapesJsAsset asset)
        {
            string path = asset.Source;

            if (!path.Contains("Media/Uploads"))
            {
                return BadRequest("Cannot delete specified asset.");
            }

            path = path.RightOf("/Media").Replace("/", "\\");
            path = Path.Combine(hostingEnvironment.WebRootPath, path);

            if (!System.IO.File.Exists(path))
            {
                return BadRequest("Specified file does not exist.");
            }

            System.IO.File.Delete(path);

            return Ok();
        }

        #endregion Asset Manager
    }
}