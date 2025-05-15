namespace Mantle.Web.Mvc;

public class MantleController : Controller
{
    public ILogger Logger { get; private set; }

    public IStringLocalizer T { get; private set; }

    public IWorkContext WorkContext { get; private set; }

    public Lazy<SiteSettings> SiteSettings { get; private set; }

    protected MantleController()
    {
        WorkContext = DependoResolver.Instance.Resolve<IWorkContext>();
        T = DependoResolver.Instance.Resolve<IStringLocalizer>();
        var loggerFactory = DependoResolver.Instance.Resolve<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger(GetType());
        SiteSettings = new Lazy<SiteSettings>(() => DependoResolver.Instance.Resolve<SiteSettings>());
    }

    protected virtual bool CheckPermission(Permission permission)
    {
        if (permission == null)
        {
            return true;
        }

        var authorizationService = DependoResolver.Instance.Resolve<IMantleAuthorizationService>();
        return authorizationService.TryCheckAccess(permission, WorkContext.CurrentUser);
    }

    protected virtual IActionResult RedirectToHomePage() => RedirectToAction("Index", "Home", new { area = string.Empty });
}