using Mantle.Web.ContentManagement.Areas.Admin.Blog.Entities;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;

public interface IBlogTagService : IGenericDataService<BlogTag>
{
}

public class BlogTagService : GenericDataService<BlogTag>, IBlogTagService
{
    public BlogTagService(ICacheManager cacheManager, IRepository<BlogTag> repository)
        : base(cacheManager, repository)
    {
    }
}