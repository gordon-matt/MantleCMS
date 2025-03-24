using Mantle.Web.ContentManagement.Areas.Admin.Blog.Entities;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Controllers.Api;

public class BlogCategoryApiController : GenericTenantODataController<BlogCategory, int>
{
    public BlogCategoryApiController(IBlogCategoryService service)
        : base(service)
    {
    }

    protected override int GetId(BlogCategory entity) => entity.Id;

    protected override void SetNewId(BlogCategory entity)
    {
    }

    protected override Permission ReadPermission => CmsPermissions.BlogRead;

    protected override Permission WritePermission => CmsPermissions.BlogWrite;
}