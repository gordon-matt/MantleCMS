using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Infrastructure;
using Mantle.Localization.Domain;
using Mantle.Localization.Services;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Domain;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services;
using Mantle.Web.OData;
using Mantle.Web.Security.Membership.Permissions;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Controllers.Api
{
    //[Authorize(Roles = MantleConstants.Roles.Administrators)]
    [Route("api/blocks/content-blocks")]
    public class ContentBlockApiController : GenericODataController<ContentBlock, Guid>
    {
        private readonly Lazy<ILocalizablePropertyService> localizablePropertyService;

        public ContentBlockApiController(
            IContentBlockService service,
            Lazy<ILocalizablePropertyService> localizablePropertyService)
            : base(service)
        {
            this.localizablePropertyService = localizablePropertyService;
        }

        protected override Guid GetId(ContentBlock entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(ContentBlock entity)
        {
            entity.Id = Guid.NewGuid();
        }

        //[EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
        public override async Task<IEnumerable<ContentBlock>> Get(ODataQueryOptions<ContentBlock> options)
        {
            if (!CheckPermission(ReadPermission))
            {
                return Enumerable.Empty<ContentBlock>().AsQueryable();
            }

            var settings = new ODataValidationSettings()
            {
                AllowedQueryOptions = AllowedQueryOptions.All
            };
            options.Validate(settings);

            using (var connection = Service.OpenConnection())
            {
                var results = options.ApplyTo(connection.Query(x => x.PageId == null));
                return await (results as IQueryable<ContentBlock>).ToHashSetAsync();
            }
        }

        //[EnableQuery]
        [HttpPost]
        public virtual async Task<IEnumerable<ContentBlock>> GetByPageId(ODataQueryOptions<ContentBlock> options, ODataActionParameters parameters)
        {
            if (!CheckPermission(ReadPermission))
            {
                return Enumerable.Empty<ContentBlock>().AsQueryable();
            }

            var pageId = (Guid)parameters["pageId"];

            var settings = new ODataValidationSettings()
            {
                AllowedQueryOptions = AllowedQueryOptions.All
            };
            options.Validate(settings);

            using (var connection = Service.OpenConnection())
            {
                var query = connection
                    .Query(x => x.PageId == pageId)
                    .OrderBy(x => x.ZoneId)
                    .ThenBy(x => x.Order);

                var results = options.ApplyTo(query);
                return await (results as IQueryable<ContentBlock>).ToHashSetAsync();
            }
        }

        public override async Task<IActionResult> Put([FromODataUri] Guid key, [FromBody] ContentBlock entity)
        {
            var blockType = Type.GetType(entity.BlockType);
            var contentBlocks = EngineContext.Current.ResolveAll<IContentBlock>();
            var contentBlock = contentBlocks.First(x => x.GetType() == blockType);
            entity.BlockName = contentBlock.Name;
            return await base.Put(key, entity);
        }

        public override async Task<IActionResult> Post([FromBody] ContentBlock entity)
        {
            var blockType = Type.GetType(entity.BlockType);
            var contentBlocks = EngineContext.Current.ResolveAll<IContentBlock>();
            var contentBlock = contentBlocks.First(x => x.GetType() == blockType);
            entity.BlockName = contentBlock.Name;
            return await base.Post(entity);
        }

        [HttpGet]
        public async Task<IActionResult> GetLocalized([FromODataUri] Guid id, [FromODataUri] string cultureCode)
        {
            if (!CheckPermission(ReadPermission))
            {
                return Unauthorized();
            }

            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var entity = await Service.FindOneAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            string entityType = typeof(ContentBlock).FullName;
            string entityId = entity.Id.ToString();

            var localizedRecord = await localizablePropertyService.Value.FindOneAsync(x =>
                x.CultureCode == cultureCode &&
                x.EntityType == entityType &&
                x.EntityId == entityId &&
                x.Property == "BlockValues");

            if (localizedRecord != null)
            {
                entity.BlockValues = localizedRecord.Value;
            }

            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> SaveLocalized(ODataActionParameters parameters)
        {
            if (!CheckPermission(WritePermission))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string cultureCode = (string)parameters["cultureCode"];
            var entity = (ContentBlock)parameters["entity"];

            if (entity.Id == Guid.Empty)
            {
                return BadRequest();
            }
            string entityType = typeof(ContentBlock).FullName;
            string entityId = entity.Id.ToString();

            var localizedRecord = await localizablePropertyService.Value.FindOneAsync(x =>
                x.CultureCode == cultureCode &&
                x.EntityType == entityType &&
                x.EntityId == entityId &&
                x.Property == "BlockValues");

            if (localizedRecord == null)
            {
                localizedRecord = new LocalizableProperty
                {
                    CultureCode = cultureCode,
                    EntityType = entityType,
                    EntityId = entityId,
                    Property = "BlockValues",
                    Value = entity.BlockValues
                };
                await localizablePropertyService.Value.InsertAsync(localizedRecord);
                return Ok();
            }
            else
            {
                localizedRecord.Value = entity.BlockValues;
                await localizablePropertyService.Value.UpdateAsync(localizedRecord);
                return Ok();
            }
        }

        protected override Permission ReadPermission
        {
            get { return CmsPermissions.ContentBlocksRead; }
        }

        protected override Permission WritePermission
        {
            get { return CmsPermissions.ContentBlocksWrite; }
        }
    }
}