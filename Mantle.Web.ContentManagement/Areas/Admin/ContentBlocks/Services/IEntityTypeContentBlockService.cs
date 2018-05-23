using System;
using System.Collections.Generic;
using System.Linq;
using Extenso;
using Extenso.Data.Entity;
using Mantle.Caching;
using Mantle.Data.Services;
using Mantle.Localization.Services;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Domain;
using Microsoft.Extensions.Logging;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services
{
    public interface IEntityTypeContentBlockService : IGenericDataService<EntityTypeContentBlock>
    {
        IEnumerable<IContentBlock> GetContentBlocks(string entityType, string entityId, string zoneName, string cultureCode, bool includeDisabled = false);
    }

    public class EntityTypeContentBlockService : GenericDataService<EntityTypeContentBlock>, IEntityTypeContentBlockService
    {
        private readonly Lazy<IRepository<Zone>> zoneRepository;
        private readonly Lazy<ILocalizablePropertyService> localizablePropertyService;
        private readonly IWorkContext workContext;

        public EntityTypeContentBlockService(
            ICacheManager cacheManager,
            IRepository<EntityTypeContentBlock> repository,
            Lazy<IRepository<Zone>> zoneRepository,
            Lazy<ILocalizablePropertyService> localizablePropertyService,
            IWorkContext workContext)
            : base(cacheManager, repository)
        {
            this.zoneRepository = zoneRepository;
            this.localizablePropertyService = localizablePropertyService;
            this.workContext = workContext;
        }

        //TODO: Override other Delete() methods with similar logic to this one
        public override int Delete(EntityTypeContentBlock entity)
        {
            string entityType = typeof(EntityTypeContentBlock).FullName;
            string entityId = entity.Id.ToString();

            var localizedRecords = localizablePropertyService.Value.Find(x =>
                x.EntityType == entityType &&
                x.EntityId == entityId);

            int rowsAffected = localizablePropertyService.Value.Delete(localizedRecords);
            rowsAffected += base.Delete(entity);

            return rowsAffected;
        }

        #region IEntityTypeContentBlockService Members

        public IEnumerable<IContentBlock> GetContentBlocks(string entityType, string entityId, string zoneName, string cultureCode, bool includeDisabled = false)
        {
            int tenantId = GetTenantId();

            string key = string.Format(
                "Repository_EntityTypeContentBlocks_GetContentBlocks_{0}_{1}_{2}_{3}_{4}_{5}",
                tenantId,
                entityType,
                cultureCode,
                entityId,
                zoneName,
                includeDisabled);

            var records = CacheManager.Get(key, () =>
            {
                var zone = zoneRepository.Value.FindOne(x => x.TenantId == tenantId && x.Name == zoneName);

                if (zone == null)
                {
                    zoneRepository.Value.Insert(new Zone
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        Name = zoneName
                    });
                    return Enumerable.Empty<EntityTypeContentBlock>();
                }

                using (var connection = OpenConnection())
                {
                    var query = connection.Query(x =>
                        x.EntityType == entityType &&
                        x.EntityId == entityId &&
                        x.ZoneId == zone.Id);

                    if (!includeDisabled)
                    {
                        query = query.Where(x => x.IsEnabled);
                    }

                    return query.ToList();
                }
            });

            return GetContentBlocks(records, cultureCode);
        }

        #endregion IEntityTypeContentBlockService Members

        protected override void ClearCache()
        {
            base.ClearCache();
            CacheManager.RemoveByPattern("Repository_EntityTypeContentBlocks_GetContentBlocks_.*");
        }

        private IEnumerable<IContentBlock> GetContentBlocks(IEnumerable<EntityTypeContentBlock> records, string cultureCode)
        {
            string entityType = typeof(EntityTypeContentBlock).FullName;
            var ids = records.Select(x => x.Id.ToString());

            var localizedRecords = localizablePropertyService.Value.Find(x =>
                x.CultureCode == cultureCode &&
                x.EntityType == entityType &&
                ids.Contains(x.EntityId) &&
                x.Property == "BlockValues");

            var result = new List<IContentBlock>();
            foreach (var record in records)
            {
                IContentBlock contentBlock;
                try
                {
                    if (!string.IsNullOrEmpty(cultureCode))
                    {
                        string entityId = record.Id.ToString();

                        var localizedRecord = localizedRecords.FirstOrDefault(x => x.EntityId == entityId);
                        if (localizedRecord != null)
                        {
                            record.BlockValues = localizedRecord.Value;
                        }
                    }

                    var blockType = Type.GetType(record.BlockType);
                    contentBlock = (IContentBlock)record.BlockValues.JsonDeserialize(blockType);
                }
                catch (Exception x)
                {
                    Logger.LogError(new EventId(), x, x.Message);
                    continue;
                }

                contentBlock.Id = record.Id;
                contentBlock.Title = record.Title;
                contentBlock.ZoneId = record.ZoneId;
                contentBlock.Order = record.Order;
                contentBlock.Enabled = record.IsEnabled;
                contentBlock.CustomTemplatePath = record.CustomTemplatePath;
                result.Add(contentBlock);
            }
            return result;
        }

        protected virtual int GetTenantId()
        {
            return workContext.CurrentTenant.Id;
        }
    }
}