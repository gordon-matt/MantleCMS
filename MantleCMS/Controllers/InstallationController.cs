using Mantle.Web.Installation;
using Mantle.Web.Models;
using MantleCMS.Data;
using Microsoft.AspNetCore.Mvc;

namespace MantleCMS.Controllers
{
    [Route("installation")]
    public class InstallationController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            var model = new InstallationModel
            {
                AdminEmail = "admin@yourSite.com",
                //DefaultLanguage = "en-US"
            };
            return View(model);
        }

        [HttpPost]
        [Route("post-install")]
        public IActionResult PostInstall(InstallationModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            InstallationHelper.Install<ApplicationDbContext>(model);

            return RedirectToAction("Index", "Home");
        }
    }
}