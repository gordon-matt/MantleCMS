using System;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.Pages.Services;
using Mantle.Web.OData;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.Areas.Admin.Pages.Controllers.Api
{
    [Route("api/page-types")]
    public class PageTypeApiController : GenericODataController<PageType, Guid>
    {
        public PageTypeApiController(IPageTypeService service)
            : base(service)
        {
        }

        protected override Guid GetId(PageType entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(PageType entity)
        {
            entity.Id = Guid.NewGuid();
        }

        protected override Permission ReadPermission
        {
            get { return CmsPermissions.PageTypesRead; }
        }

        protected override Permission WritePermission
        {
            get { return CmsPermissions.PageTypesWrite; }
        }
    }
}