using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Controllers;

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
    public IActionResult Index() => !CheckPermission(CmsPermissions.ContentBlocksRead)
        ? Unauthorized()
        : PartialView("/Areas/Admin/ContentBlocks/Views/ContentBlock/Index.cshtml");

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