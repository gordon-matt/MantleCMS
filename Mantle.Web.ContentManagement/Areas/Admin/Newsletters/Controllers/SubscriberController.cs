using Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Services;
using Mantle.Web.Security.Membership;

namespace Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Controllers;

[Authorize]
[Area(CmsConstants.Areas.Newsletters)]
[Route("admin/newsletters/subscribers")]
public class SubscriberController : MantleController
{
    private readonly Lazy<IMembershipService> membershipService;
    private readonly Lazy<MembershipSettings> membershipSettings;
    private readonly Lazy<INewsletterService> newsletterService;

    public SubscriberController(
        Lazy<INewsletterService> newsletterService,
        Lazy<IMembershipService> membershipService,
        Lazy<MembershipSettings> membershipSettings)
    {
        this.newsletterService = newsletterService;
        this.membershipService = membershipService;
        this.membershipSettings = membershipSettings;
    }

    //[OutputCache(Duration = 86400, VaryByParam = "none")]
    [Route("")]
    public IActionResult Index()
    {
        if (!CheckPermission(CmsPermissions.NewsletterRead))
        {
            return Unauthorized();
        }

        WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Newsletters.Title].Value);
        WorkContext.Breadcrumbs.Add(T[MantleCmsLocalizableStrings.Newsletters.Subscribers].Value);

        ViewBag.Title = T[MantleCmsLocalizableStrings.Newsletters.Title].Value;
        ViewBag.SubTitle = T[MantleCmsLocalizableStrings.Newsletters.Subscribers].Value;

        return PartialView();
    }

    [AllowAnonymous]
    [Route("subscribe")]
    [ValidateAntiForgeryToken]
    public JsonResult Subscribe(string email, string name)
    {
        string message = string.Empty;
        bool success = newsletterService.Value.Subscribe(email, name, WorkContext.CurrentUser, out message);

        return Json(new
        {
            Success = success,
            Message = message
        });
    }

    [Route("download-csv")]
    public async Task<FileContentResult> DownloadCsv()
    {
        var userIds = (await membershipService.Value
            .GetProfileEntriesByKeyAndValue(WorkContext.CurrentTenant.Id, NewsletterUserProfileProvider.Fields.SubscribeToNewsletters, "true"))
            .Select(x => x.UserId);

        var users = (await membershipService.Value.GetUsers(WorkContext.CurrentTenant.Id, x => userIds.Contains(x.Id)))
            .Select(x => new
            {
                Email = x.Email,
                Name = membershipService.Value.GetUserDisplayName(x)
            })
            .OrderBy(x => x.Name);

        string csv = users.ToCsv();
        string fileName = string.Format("Subscribers_{0:yyyy_MM_dd}.csv", DateTime.Now);
        return File(new UTF8Encoding().GetBytes(csv), "text/csv", fileName);
    }
}