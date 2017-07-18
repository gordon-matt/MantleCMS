using System.Linq;
using System.Threading.Tasks;
using Mantle.Security.Membership;
using Mantle.Web.Mvc.KendoUI;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Mantle.Web.Areas.Admin.Membership.Controllers
{
    [Route("api/membership/users-public")]
    public class PublicUserApiController : Controller
    {
        private readonly IWorkContext workContext;
        protected IMembershipService Service { get; private set; }

        public PublicUserApiController(IMembershipService service, IWorkContext workContext)
        {
            this.Service = service;
            this.workContext = workContext;
        }

        [HttpPost]
        [Route("get")]
        public virtual async Task<IActionResult> Get([FromBody]KendoGridMvcRequest request)
        {
            var query = (await Service.GetAllUsers(workContext.CurrentTenant.Id))
                .Select(x => new PublicUserInfo
                {
                    Id = x.Id,
                    UserName = x.UserName
                });

            var grid = new CustomKendoGridEx<PublicUserInfo>(request, query);
            return Json(grid);
        }

        [HttpGet]
        [Route("{key}")]
        public virtual async Task<IActionResult> Get(string key)
        {
            var entity = await Service.GetUserById(key);
            return Json(JObject.FromObject(new PublicUserInfo
            {
                Id = entity.Id,
                UserName = entity.UserName
            }));
        }
    }

    public class PublicUserInfo
    {
        public string Id { get; set; }

        public string UserName { get; set; }
    }
}