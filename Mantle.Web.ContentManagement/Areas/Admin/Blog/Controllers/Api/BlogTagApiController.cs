﻿using Mantle.Security.Membership.Permissions;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Blog.Services;
using Mantle.Web.OData;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog.Controllers.Api
{
    public class BlogTagApiController : GenericTenantODataController<BlogTag, int>
    {
        public BlogTagApiController(IBlogTagService service)
            : base(service)
        {
        }

        protected override int GetId(BlogTag entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(BlogTag entity)
        {
        }

        protected override Permission ReadPermission
        {
            get { return CmsPermissions.BlogRead; }
        }

        protected override Permission WritePermission
        {
            get { return CmsPermissions.BlogWrite; }
        }
    }
}