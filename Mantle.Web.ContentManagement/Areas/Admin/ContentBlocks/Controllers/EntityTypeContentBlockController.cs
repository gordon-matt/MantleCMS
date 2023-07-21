using Extenso.AspNetCore.Mvc.Rendering;
using Mantle.Infrastructure;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;
using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Controllers
{
    [Authorize]
    [Area(CmsConstants.Areas.Blocks)]
    [Route("admin/blocks/entity-type-content-blocks")]
    public class EntityTypeContentBlockController : MantleController
    {
        private readonly Lazy<IEntityTypeContentBlockService> entityTypeContentBlockService;
        private readonly Lazy<IRazorViewRenderService> razorViewRenderService;

        public EntityTypeContentBlockController(
            Lazy<IEntityTypeContentBlockService> entityTypeContentBlockService,
            Lazy<IRazorViewRenderService> razorViewRenderService)
        {
            this.entityTypeContentBlockService = entityTypeContentBlockService;
            this.razorViewRenderService = razorViewRenderService;
        }

        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("")]
        public IActionResult Index()
        {
            if (!CheckPermission(CmsPermissions.ContentBlocksRead))
            {
                return Unauthorized();
            }

            WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value);

            ViewBag.Title = T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value;
            ViewBag.SubTitle = T[MantleCmsLocalizableStrings.ContentBlocks.ManageContentBlocks].Value;

            return PartialView("Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Views.EntityTypeContentBlock.Index");
        }

        [Route("get-view-data")]
        public JsonResult GetViewData()
        {
            return Json(new
            {
                gridPageSize = SiteSettings.Value.DefaultGridPageSize,
                translations = new
                {
                    create = T[MantleWebLocalizableStrings.General.Create].Value,
                    delete = T[MantleWebLocalizableStrings.General.Delete].Value,
                    deleteRecordConfirm = T[MantleWebLocalizableStrings.General.ConfirmDeleteRecord].Value,
                    deleteRecordError = T[MantleWebLocalizableStrings.General.DeleteRecordError].Value,
                    deleteRecordSuccess = T[MantleWebLocalizableStrings.General.DeleteRecordSuccess].Value,
                    edit = T[MantleWebLocalizableStrings.General.Edit].Value,
                    getRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                    insertRecordError = T[MantleWebLocalizableStrings.General.InsertRecordError].Value,
                    insertRecordSuccess = T[MantleWebLocalizableStrings.General.InsertRecordSuccess].Value,
                    localize = T[MantleWebLocalizableStrings.General.Localize].Value,
                    toggle = T[MantleWebLocalizableStrings.General.Toggle].Value,
                    updateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                    updateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                    columns = new
                    {
                        title = T[MantleCmsLocalizableStrings.ContentBlocks.Model.Title].Value,
                        blockType = T[MantleCmsLocalizableStrings.ContentBlocks.Model.BlockType].Value,
                        order = T[MantleCmsLocalizableStrings.ContentBlocks.Model.Order].Value,
                        isEnabled = T[MantleCmsLocalizableStrings.ContentBlocks.Model.IsEnabled].Value,
                        name = T[MantleCmsLocalizableStrings.ContentBlocks.ZoneModel.Name].Value,
                    }
                }
            });
        }

        [Route("get-editor-ui/{contentBlockId}")]
        public async Task<IActionResult> GetEditorUI(Guid contentBlockId)
        {
            var blockEntity = entityTypeContentBlockService.Value.FindOne(contentBlockId);
            var blockType = Type.GetType(blockEntity.BlockType);

            var blocks = EngineContext.Current.ResolveAll<IContentBlock>();
            var block = blocks.First(x => x.GetType() == blockType);

            string content;

            try
            {
                // TODO: See if we can make EditorTemplatePath not so specific a path (just the name), so we can override it in themes, etc
                content = await razorViewRenderService.Value.RenderToStringAsync(block.EditorTemplatePath, block);
            }
            catch (NotSupportedException)
            {
                content = string.Empty;
            }

            return Json(new { Content = content });
        }
    }
}