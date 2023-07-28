namespace Mantle.Web.Areas.Admin.Configuration.Controllers;

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

    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("get-translations")]
    public JsonResult GetTranslations()
    {
        return Json(new
        {
            Edit = T[MantleWebLocalizableStrings.General.Edit].Value,
            GetRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
            UpdateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
            UpdateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
            Columns = new
            {
                Name = T[MantleWebLocalizableStrings.Settings.Model.Name].Value,
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