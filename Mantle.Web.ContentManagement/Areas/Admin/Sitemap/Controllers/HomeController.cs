using Mantle.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Controllers
{
    public class HomeController : MantleController
    {
        private readonly IWebHelper webHelper;

        public HomeController(IWebHelper webHelper)
        {
            this.webHelper = webHelper;
        }

        [Route("sitemap.xml")]
        public IActionResult SitemapXml()
        {
            int tenantId = WorkContext.CurrentTenant.Id;
            string filePath = webHelper.MapPath(string.Format("~/sitemap-{0}.xml", tenantId));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            string fileContent = System.IO.File.ReadAllText(filePath);
            return Content(fileContent, "text/xml");
        }
    }
}