namespace Mantle.Web.Areas.Admin.Configuration.Controllers;

[Authorize]
[Area(MantleWebConstants.Areas.Configuration)]
[Route("admin/configuration/settings")]
public class SettingsController : MantleController
{
    private readonly Lazy<IEnumerable<ISettings>> settings;
    private readonly Lazy<IRazorViewRenderService> razorViewRenderService;

    public SettingsController(
        Lazy<IEnumerable<ISettings>> settings,
        Lazy<IRazorViewRenderService> razorViewRenderService)
        : base()
    {
        this.settings = settings;
        this.razorViewRenderService = razorViewRenderService;
    }

    [Route("")]
    public IActionResult Index() => !CheckPermission(MantleWebPermissions.SettingsRead) ? Unauthorized() : PartialView();

    [Route("get-editor-ui/{type}")]
    public async Task<IActionResult> GetEditorUI(string type)
    {
        var model = settings.Value.FirstOrDefault(x => x.GetType().FullName == type.Replace('-', '.'));

        if (model == null)
        {
            return NotFound();
        }

        string content = await razorViewRenderService.Value.RenderToStringAsync(model.EditorTemplatePath, model);
        return Json(new { Content = content });
    }
}