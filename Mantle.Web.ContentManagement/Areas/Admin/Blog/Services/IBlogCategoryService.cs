using Extenso.Data.Entity;
using Mantle.Caching;
using Mantle.Data.Services;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Domain;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Services
{
    public interface IBlogCategoryService : IGenericDataService<BlogCategory>
    {
    }

    public class BlogCategoryService : GenericDataService<BlogCategory>, IBlogCategoryService
    {
        public BlogCategoryService(ICacheManager cacheManager, IRepository<BlogCategory> repository)
            : base(cacheManager, repository)
        {
        }
    }
}