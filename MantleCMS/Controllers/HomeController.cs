namespace MantleCMS.Controllers;

//[Authorize]
public class HomeController : MantleController
{
    private readonly SiteSettings siteSettings;

    public HomeController(SiteSettings siteSettings)
    {
        this.siteSettings = siteSettings;
    }

    [Route("")]
    public IActionResult Index()
    {
        if (!DataSettingsHelper.IsDatabaseInstalled)
        {
            return RedirectToAction("Index", "Installation");
        }

        ViewBag.Title = siteSettings.HomePageTitle;
        return View();
    }

    [Route("about")]
    public IActionResult About() => View();

    [Route("contact")]
    public IActionResult Contact() => View();

    [Route("test")]
    public IActionResult Test() => View();
}