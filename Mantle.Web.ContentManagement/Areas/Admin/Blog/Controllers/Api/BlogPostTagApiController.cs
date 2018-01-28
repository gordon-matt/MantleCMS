using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Controllers.Api
{
    [Route("api/blog/post-tags")]
    public class BlogPostTagApiController : ODataController
    {
        // Here only for the purpose of making Web API happy (if this is not here, it complains because Post and Tag refer to PostTag)
    }
}