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
    [Route("admin/blocks/content-blocks")]
    public class ContentBlockController : MantleController
    {
        private readonly Lazy<IContentBlockService> contentBlockService;
        private readonly Lazy<IRazorViewRenderService> razorViewRenderService;

        public ContentBlockController(
            Lazy<IContentBlockService> contentBlockService,
            Lazy<IRazorViewRenderService> razorViewRenderService)
        {
            this.contentBlockService = contentBlockService;
            this.razorViewRenderService = razorViewRenderService;
        }

        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("{pageId?}")]
        public IActionResult Index()
        {
            if (!CheckPermission(CmsPermissions.ContentBlocksRead))
            {
                return Unauthorized();
            }

            WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value);

            ViewBag.Title = T[MantleCmsLocalizableStrings.ContentBlocks.Title].Value;
            ViewBag.SubTitle = T[MantleCmsLocalizableStrings.ContentBlocks.ManageContentBlocks].Value;

            //if (pageId.HasValue)
            //{
            //    WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Pages.ManagePages].Value, Url.Action("Index", "Page", new { area = CmsConstants.Areas.Pages }));
            //}

            //ViewBag.PageId = pageId;

            return PartialView("Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Views.ContentBlock.Index");
        }

        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("get-translations")]
        public JsonResult GetTranslations()
        {
            return Json(new
            {
                Create = T[MantleWebLocalizableStrings.General.Create].Value,
                Delete = T[MantleWebLocalizableStrings.General.Delete].Value,
                DeleteRecordConfirm = T[MantleWebLocalizableStrings.General.ConfirmDeleteRecord].Value,
                DeleteRecordError = T[MantleWebLocalizableStrings.General.DeleteRecordError].Value,
                DeleteRecordSuccess = T[MantleWebLocalizableStrings.General.DeleteRecordSuccess].Value,
                Edit = T[MantleWebLocalizableStrings.General.Edit].Value,
                GetRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                InsertRecordError = T[MantleWebLocalizableStrings.General.InsertRecordError].Value,
                InsertRecordSuccess = T[MantleWebLocalizableStrings.General.InsertRecordSuccess].Value,
                Localize = T[MantleWebLocalizableStrings.General.Localize].Value,
                Toggle = T[MantleWebLocalizableStrings.General.Toggle].Value,
                UpdateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                UpdateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                Columns = new
                {
                    Title = T[MantleCmsLocalizableStrings.ContentBlocks.Model.Title].Value,
                    BlockType = T[MantleCmsLocalizableStrings.ContentBlocks.Model.BlockType].Value,
                    Order = T[MantleCmsLocalizableStrings.ContentBlocks.Model.Order].Value,
                    IsEnabled = T[MantleCmsLocalizableStrings.ContentBlocks.Model.IsEnabled].Value,
                    Name = T[MantleCmsLocalizableStrings.ContentBlocks.ZoneModel.Name].Value,
                }
            });
        }

        [Route("get-editor-ui/{contentBlockId}")]
        public async Task<IActionResult> GetEditorUI(Guid contentBlockId)
        {
            var blockEntity = contentBlockService.Value.FindOne(contentBlockId);
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