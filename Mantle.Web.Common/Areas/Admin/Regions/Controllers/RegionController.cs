using Mantle.Web.Common.Areas.Admin.Regions.Entities;
using Mantle.Web.Common.Areas.Admin.Regions.Services;

namespace Mantle.Web.Common.Areas.Admin.Regions.Controllers;

[Authorize]
[Area(Constants.Areas.Regions)]
[Route("admin/regions")]
public class RegionController : MantleController
{
    private readonly Lazy<IRegionService> regionService;
    private readonly Lazy<IEnumerable<IRegionSettings>> regionSettings;
    private readonly Lazy<IRazorViewRenderService> razorViewRenderService;

    public RegionController(
        Lazy<IRazorViewRenderService> razorViewRenderService,
        Lazy<IRegionService> regionService,
        Lazy<IEnumerable<IRegionSettings>> regionSettings)
    {
        this.razorViewRenderService = razorViewRenderService;
        this.regionService = regionService;
        this.regionSettings = regionSettings;
    }

    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index()
    {
        if (!CheckPermission(Permissions.RegionsRead))
        {
            return Unauthorized();
        }

        WorkContext.Breadcrumbs.Add(T[LocalizableStrings.Regions.Title]);

        ViewBag.Title = T[LocalizableStrings.Regions.Title];
        ViewBag.RegionSettings = regionSettings.Value;

        //var model = regionService.Value.GetContinents(true).Select(x => (RegionModel)x);
        return PartialView();
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
            Settings = T[MantleWebLocalizableStrings.General.Settings].Value,
            UpdateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
            UpdateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
            Cities = T[LocalizableStrings.Regions.Cities].Value,
            States = T[LocalizableStrings.Regions.States].Value,
            Columns = new
            {
                Name = T[LocalizableStrings.Regions.Model.Name].Value
            }
        });
    }

    [Route("get-editor-ui/{settingsId}")]
    public async Task<IActionResult> GetEditorUI(string settingsId)
    {
        var dictionary = regionSettings.Value.ToDictionary(k => k.Name.ToSlugUrl(), v => v);

        if (!dictionary.ContainsKey(settingsId))
        {
            return Json(new { Content = string.Empty });
        }

        var model = dictionary[settingsId];

        if (model == null)
        {
            return NotFound();
        }

        //WorkContext.Breadcrumbs.Add(T[LocalizableStrings.Regions.Title), Url.Action("Index", new { area = Constants.Areas.Regions }));
        //WorkContext.Breadcrumbs.Add(model.Name);
        //WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.General.Edit));

        string content = await razorViewRenderService.Value.RenderToStringAsync(model.EditorTemplatePath, model);
        return Json(new { Content = content });
    }

    [AllowAnonymous]
    [Route("get-states/{countryId}")]
    public JsonResult GetStates(int countryId)
    {
        var states = regionService.Value
            .GetStates(countryId, WorkContext.CurrentCultureCode)
            .ToDictionary(k => k.Id, v => v);

        if (!states.Any())
        {
            return Json(new { Data = new[] { new { Id = -1, Name = "N/A" } } });
        }

        var data = states.Values
            .OrderBy(x => x.Order == null)
            .ThenBy(x => x.Order)
            .ThenBy(x => x.Name)
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Name
            });

        return Json(new { Data = data });
    }

    [AllowAnonymous]
    [Route("get-cities/{regionId}")]
    public JsonResult GetCities(int regionId)
    {
        var cities = regionService.Value
            .GetSubRegions(regionId, RegionType.City, WorkContext.CurrentCultureCode)
            .ToDictionary(k => k.Id, v => v);

        var data = cities.Values
            .OrderBy(x => x.Order == null)
            .ThenBy(x => x.Order)
            .ThenBy(x => x.Name)
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Name
            });

        return Json(new { Data = data });
    }
}