﻿using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Controllers;

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
    public IActionResult Index() => !CheckPermission(CmsPermissions.ContentBlocksRead)
            ? Unauthorized()
            : PartialView("/Areas/Admin/ContentBlocks/Views/EntityTypeContentBlock/Index.cshtml");

    [Route("get-editor-ui/{contentBlockId}")]
    public async Task<IActionResult> GetEditorUI(Guid contentBlockId)
    {
        var blockEntity = entityTypeContentBlockService.Value.FindOne(contentBlockId);
        var blockType = Type.GetType(blockEntity.BlockType);

        var blocks = DependoResolver.Instance.ResolveAll<IContentBlock>();
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