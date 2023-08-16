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
}