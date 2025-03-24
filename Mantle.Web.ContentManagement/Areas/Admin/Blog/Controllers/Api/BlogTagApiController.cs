using Mantle.Web.ContentManagement.Areas.Admin.Blog.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Controllers.Api;

public class BlogTagApiController : GenericTenantODataController<BlogTag, int>
{
    public BlogTagApiController(IBlogTagService service)
        : base(service)
    {
    }

    protected override int GetId(BlogTag entity) => entity.Id;

    protected override void SetNewId(BlogTag entity)
    {
    }

    protected override Permission ReadPermission => CmsPermissions.BlogRead;

    protected override Permission WritePermission => CmsPermissions.BlogWrite;
}