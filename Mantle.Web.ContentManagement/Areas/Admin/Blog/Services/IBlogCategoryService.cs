﻿using Mantle.Web.ContentManagement.Areas.Admin.Blog.Entities;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;

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