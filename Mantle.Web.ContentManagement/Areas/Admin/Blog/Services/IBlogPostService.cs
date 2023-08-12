using Mantle.Web.ContentManagement.Areas.Admin.Blog.Entities;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;

public interface IBlogPostService : IGenericDataService<BlogPost>
{
}

public class BlogPostService : GenericDataService<BlogPost>, IBlogPostService
{
    public BlogPostService(ICacheManager cacheManager, IRepository<BlogPost> repository)
        : base(cacheManager, repository)
    {
    }
}