using System;
using System.Linq;
using System.Threading.Tasks;
using Mantle.Caching;
using Mantle.Localization.Domain;
using Mantle.Localization.Services;
using Mantle.Web.Areas.Admin.Localization.Models;
using Mantle.Web.Mvc;
using Mantle.Web.Mvc.KendoUI;
using Mantle.Web.Security.Membership.Permissions;
using KendoGridBinderEx.ModelBinder.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Mantle.Web.Areas.Admin.Localization.Controllers
{
    [Area(MantleWebConstants.Areas.Localization)]
    [Route("api/localization/localizable-strings")]
    public class LocalizableStringApiController : MantleGenericTenantDataController<LocalizableString, Guid>
    {
        private readonly ICacheManager cacheManager;

        public LocalizableStringApiController(
            ILocalizableStringService service,
            ICacheManager cacheManager)
            : base(service)
        {
            this.cacheManager = cacheManager;
        }

        [HttpPost]
        [Route("get")]
        public override async Task<IActionResult> Get([FromBody]KendoGridMvcRequest request)
        {
            return await base.Get(request);
        }

        [HttpGet]
        [Route("{key}")]
        public override async Task<IActionResult> Get(Guid key)
        {
            return await base.Get(key);
        }

        [HttpPut]
        [Route("{key}")]
        public override async Task<IActionResult> Put(Guid key, [FromBody]LocalizableString entity)
        {
            return await base.Put(key, entity);
        }

        [HttpPost]
        [Route("")]
        public override async Task<IActionResult> Post([FromBody]LocalizableString entity)
        {
            return await base.Post(entity);
        }

        [HttpDelete]
        [Route("{key}")]
        public override async Task<IActionResult> Delete(Guid key)
        {
            return await base.Delete(key);
        }

        [HttpGet]
        [Route("Default.GetComparitiveTable/{cultureCode}")]
        public virtual async Task<IActionResult> GetComparitiveTable(string cultureCode, [FromBody]dynamic data)
        {
            if (!CheckPermission(ReadPermission))
            {
                return Unauthorized();
            }
            else
            {
                int tenantId = GetTenantId();
                using (var connection = Service.OpenConnection())
                {
                    // With grouping, we use .Where() and then .FirstOrDefault() instead of just the .FirstOrDefault() by itself
                    //  for compatibility with MySQL.
                    //  See: http://stackoverflow.com/questions/23480044/entity-framework-select-statement-with-logic
                    var query = connection.Query(x => x.TenantId == tenantId && (x.CultureCode == null || x.CultureCode == cultureCode))
                            .GroupBy(x => x.TextKey)
                            .Select(grp => new ComparitiveLocalizableString
                            {
                                Key = grp.Key,
                                InvariantValue = grp.Where(x => x.CultureCode == null).FirstOrDefault().TextValue,
                                LocalizedValue = grp.Where(x => x.CultureCode == cultureCode).FirstOrDefault() == null
                                    ? string.Empty
                                    : grp.Where(x => x.CultureCode == cultureCode).FirstOrDefault().TextValue
                            });

                    // TODO: Test
                    KendoGridMvcRequest request = data;
                    var grid = new CustomKendoGridEx<ComparitiveLocalizableString>(request, query);
                    return Json(grid);
                }
            }
        }

        [HttpPost]
        [Route("Default.PutComparitive")]
        public virtual async Task<IActionResult> PutComparitive([FromBody]dynamic data)
        {
            if (!CheckPermission(WritePermission))
            {
                return Unauthorized();
            }

            string cultureCode = data.cultureCode;
            string key = data.key;
            ComparitiveLocalizableString entity = data.entity;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!key.Equals(entity.Key))
            {
                return BadRequest();
            }

            int tenantId = GetTenantId();
            var localizedString = await Service.FindOneAsync(x => x.TenantId == tenantId && x.CultureCode == cultureCode && x.TextKey == key);

            if (localizedString == null)
            {
                localizedString = new LocalizableString
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    CultureCode = cultureCode,
                    TextKey = key,
                    TextValue = entity.LocalizedValue
                };
                await Service.InsertAsync(localizedString);
            }
            else
            {
                localizedString.TextValue = entity.LocalizedValue;
                await Service.UpdateAsync(localizedString);
            }

            cacheManager.Remove(string.Concat(MantleConstants.CacheKeys.LocalizableStringsFormat, tenantId, cultureCode));

            return Ok(entity);
        }

        [HttpPost]
        [Route("Default.DeleteComparitive")]
        public virtual async Task<IActionResult> DeleteComparitive([FromBody]dynamic data)
        {
            if (!CheckPermission(WritePermission))
            {
                return Unauthorized();
            }

            string cultureCode = data.cultureCode;
            string key = data.key;

            int tenantId = GetTenantId();
            var entity = await Service.FindOneAsync(x => x.TenantId == tenantId && x.CultureCode == cultureCode && x.TextKey == key);
            if (entity == null)
            {
                return NotFound();
            }

            entity.TextValue = null;
            await Service.UpdateAsync(entity);
            //Repository.Delete(entity);

            cacheManager.Remove(string.Concat(MantleConstants.CacheKeys.LocalizableStringsFormat, tenantId, cultureCode));

            return NoContent();
        }

        protected override Guid GetId(LocalizableString entity)
        {
            return entity.Id;
        }

        protected override void SetNewId(LocalizableString entity)
        {
            entity.Id = Guid.NewGuid();
        }

        protected override Permission ReadPermission
        {
            get { return MantleWebPermissions.LocalizableStringsRead; }
        }

        protected override Permission WritePermission
        {
            get { return MantleWebPermissions.LocalizableStringsWrite; }
        }
    }
}