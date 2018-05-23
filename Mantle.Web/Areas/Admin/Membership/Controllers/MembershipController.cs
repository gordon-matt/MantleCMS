using System.Threading.Tasks;
using Mantle.Security.Membership;
using Mantle.Web.Collections;
using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Areas.Admin.Membership.Controllers
{
    [Authorize]
    [Area(MantleWebConstants.Areas.Membership)]
    [Route("admin/membership")]
    public class MembershipController : MantleController
    {
        private readonly IMembershipService membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            this.membershipService = membershipService;
        }

        protected virtual bool CheckPermissions()
        {
            return CheckPermission(MantleWebPermissions.MembershipManage);
        }

        //[OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("")]
        public virtual async Task<IActionResult> Index()
        {
            if (!CheckPermissions())
            {
                return Unauthorized();
            }

            WorkContext.Breadcrumbs.Add(T[MantleWebLocalizableStrings.Membership.Title].Value);

            ViewBag.Title = T[MantleWebLocalizableStrings.Membership.Title].Value;

            ViewBag.SelectList = (await membershipService.GetAllRoles(WorkContext.CurrentTenant.Id))
                .ToSelectList(v => v.Id.ToString(), t => t.Name, T[MantleWebLocalizableStrings.Membership.AllRolesSelectListOption].Value);

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
                    changePasswordError = T[MantleWebLocalizableStrings.Membership.ChangePasswordError].Value,
                    changePasswordSuccess = T[MantleWebLocalizableStrings.Membership.ChangePasswordSuccess].Value,
                    create = T[MantleWebLocalizableStrings.General.Create].Value,
                    delete = T[MantleWebLocalizableStrings.General.Delete].Value,
                    deleteRecordConfirm = T[MantleWebLocalizableStrings.General.ConfirmDeleteRecord].Value,
                    deleteRecordError = T[MantleWebLocalizableStrings.General.DeleteRecordError].Value,
                    deleteRecordSuccess = T[MantleWebLocalizableStrings.General.DeleteRecordSuccess].Value,
                    edit = T[MantleWebLocalizableStrings.General.Edit].Value,
                    getRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
                    insertRecordError = T[MantleWebLocalizableStrings.General.InsertRecordError].Value,
                    insertRecordSuccess = T[MantleWebLocalizableStrings.General.InsertRecordSuccess].Value,
                    password = T[MantleWebLocalizableStrings.Membership.Password].Value,
                    permissions = T[MantleWebLocalizableStrings.Membership.Permissions].Value,
                    roles = T[MantleWebLocalizableStrings.Membership.Roles].Value,
                    savePermissionsError = T[MantleWebLocalizableStrings.Membership.SavePermissionsError].Value,
                    savePermissionsSuccess = T[MantleWebLocalizableStrings.Membership.SavePermissionsSuccess].Value,
                    saveRolesError = T[MantleWebLocalizableStrings.Membership.SaveRolesError].Value,
                    saveRolesSuccess = T[MantleWebLocalizableStrings.Membership.SaveRolesSuccess].Value,
                    updateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
                    updateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
                    columns = new
                    {
                        user = new
                        {
                            isActive = T[MantleWebLocalizableStrings.Membership.UserModel.IsActive].Value,
                            roles = T[MantleWebLocalizableStrings.Membership.UserModel.Roles].Value,
                            userName = T[MantleWebLocalizableStrings.Membership.UserModel.UserName].Value,
                        },
                        role = new
                        {
                            name = T[MantleWebLocalizableStrings.Membership.RoleModel.Name].Value,
                        },
                        permission = new
                        {
                            category = T[MantleWebLocalizableStrings.Membership.PermissionModel.Category].Value,
                            name = T[MantleWebLocalizableStrings.Membership.PermissionModel.Name].Value,
                        }
                    }
                }
            });
        }
    }
}