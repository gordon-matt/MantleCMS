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