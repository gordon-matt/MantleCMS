namespace Mantle.Web.Areas.Admin.Tenants.Controllers;

[Authorize]
[Area(MantleWebConstants.Areas.Tenants)]
[Route("admin/tenants")]
public class TenantController : MantleController
{
    [Route("")]
    public IActionResult Index()
    {
        if (!CheckPermission(StandardPermissions.FullAccess))
        {
            return Unauthorized();
        }

        WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.Tenants.Title].Value);

        ViewBag.Title = T[MantleWebLocalizableStrings.Tenants.Title].Value;
        ViewBag.SubTitle = T[MantleWebLocalizableStrings.Tenants.ManageTenants].Value;

        return PartialView();
    }

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
            UpdateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
            UpdateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
            Columns = new
            {
                Name = T[MantleWebLocalizableStrings.General.Name].Value
            }
        });
    }
}