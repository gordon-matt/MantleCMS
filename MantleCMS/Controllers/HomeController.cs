namespace MantleCMS.Controllers
{
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
        public IActionResult About()
        {
            return View();
        }

        [Route("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("test")]
        public IActionResult Test()
        {
            return View();
        }
    }
}