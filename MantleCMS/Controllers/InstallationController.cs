using Mantle.Web.Installation;
using Mantle.Web.Models;
using MantleCMS.Data;
using Microsoft.AspNetCore.Mvc;

namespace MantleCMS.Controllers
{
    [Route("installation")]
    public class InstallationController : Microsoft.AspNetCore.Mvc.Controller
    {
        [Route("")]
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
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
        public Microsoft.AspNetCore.Mvc.ActionResult PostInstall(InstallationModel model)
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