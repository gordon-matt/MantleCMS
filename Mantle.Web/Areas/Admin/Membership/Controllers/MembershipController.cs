namespace Mantle.Web.Areas.Admin.Membership.Controllers;

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

    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("get-translations")]
    public JsonResult GetTranslations()
    {
        return Json(new
        {
            ChangePasswordError = T[MantleWebLocalizableStrings.Membership.ChangePasswordError].Value,
            ChangePasswordSuccess = T[MantleWebLocalizableStrings.Membership.ChangePasswordSuccess].Value,
            Create = T[MantleWebLocalizableStrings.General.Create].Value,
            Delete = T[MantleWebLocalizableStrings.General.Delete].Value,
            DeleteRecordConfirm = T[MantleWebLocalizableStrings.General.ConfirmDeleteRecord].Value,
            DeleteRecordError = T[MantleWebLocalizableStrings.General.DeleteRecordError].Value,
            DeleteRecordSuccess = T[MantleWebLocalizableStrings.General.DeleteRecordSuccess].Value,
            Edit = T[MantleWebLocalizableStrings.General.Edit].Value,
            GetRecordError = T[MantleWebLocalizableStrings.General.GetRecordError].Value,
            InsertRecordError = T[MantleWebLocalizableStrings.General.InsertRecordError].Value,
            InsertRecordSuccess = T[MantleWebLocalizableStrings.General.InsertRecordSuccess].Value,
            Password = T[MantleWebLocalizableStrings.Membership.Password].Value,
            Permissions = T[MantleWebLocalizableStrings.Membership.Permissions].Value,
            Roles = T[MantleWebLocalizableStrings.Membership.Roles].Value,
            SavePermissionsError = T[MantleWebLocalizableStrings.Membership.SavePermissionsError].Value,
            SavePermissionsSuccess = T[MantleWebLocalizableStrings.Membership.SavePermissionsSuccess].Value,
            SaveRolesError = T[MantleWebLocalizableStrings.Membership.SaveRolesError].Value,
            SaveRolesSuccess = T[MantleWebLocalizableStrings.Membership.SaveRolesSuccess].Value,
            UpdateRecordError = T[MantleWebLocalizableStrings.General.UpdateRecordError].Value,
            UpdateRecordSuccess = T[MantleWebLocalizableStrings.General.UpdateRecordSuccess].Value,
            Columns = new
            {
                User = new
                {
                    IsActive = T[MantleWebLocalizableStrings.Membership.UserModel.IsActive].Value,
                    Roles = T[MantleWebLocalizableStrings.Membership.UserModel.Roles].Value,
                    UserName = T[MantleWebLocalizableStrings.Membership.UserModel.UserName].Value,
                },
                Role = new
                {
                    Name = T[MantleWebLocalizableStrings.Membership.RoleModel.Name].Value,
                },
                Permission = new
                {
                    Category = T[MantleWebLocalizableStrings.Membership.PermissionModel.Category].Value,
                    Name = T[MantleWebLocalizableStrings.Membership.PermissionModel.Name].Value,
                }
            }
        });
    }
}