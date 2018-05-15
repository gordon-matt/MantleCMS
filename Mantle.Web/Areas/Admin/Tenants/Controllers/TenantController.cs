using Mantle.Security.Membership.Permissions;
using Mantle.Web.Configuration;
using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Areas.Admin.Tenants.Controllers
{
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
                    updateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                    updateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                    columns = new
                    {
                        name = T[MantleWebLocalizableStrings.General.Name].Value
                    }
                }
            });
        }
    }
}