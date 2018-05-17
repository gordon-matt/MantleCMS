using System;
using System.Collections.Generic;
using System.Linq;
using Extenso;
using Mantle.Caching;
using Mantle.Data;
using Mantle.Data.Services;
using Mantle.Localization.Services;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Domain;
using Microsoft.Extensions.Logging;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Services
{
    public interface IContentBlockService : IGenericDataService<ContentBlock>
    {
        IEnumerable<IContentBlock> GetContentBlocks(Guid pageId, string cultureCode);

        IEnumerable<IContentBlock> GetContentBlocks(string zoneName, string cultureCode, Guid? pageId = null, bool includeDisabled = false);
    }

    public class ContentBlockService : GenericDataService<ContentBlock>, IContentBlockService
    {
        private readonly Lazy<IRepository<Zone>> zoneRepository;
        private readonly Lazy<ILocalizablePropertyService> localizablePropertyService;
        private readonly IWorkContext workContext;

        public ContentBlockService(
            ICacheManager cacheManager,
            IRepository<ContentBlock> repository,
            Lazy<IRepository<Zone>> zoneRepository,
            Lazy<ILocalizablePropertyService> localizablePropertyService,
            IWorkContext workContext)
            : base(cacheManager, repository)
        {
            this.zoneRepository = zoneRepository;
            this.localizablePropertyService = localizablePropertyService;
            this.workContext = workContext;
        }

        #region GenericDataService<ContentBlock> Overrides

        //TODO: Override other Delete() methods with similar logic to this one
        public override int Delete(ContentBlock entity)
        {
            string entityType = typeof(ContentBlock).FullName;
            string entityId = entity.Id.ToString();

            var localizedRecords = localizablePropertyService.Value.Find(x =>
                x.EntityType == entityType &&
                x.EntityId == entityId);

            int rowsAffected = localizablePropertyService.Value.Delete(localizedRecords);
            rowsAffected += base.Delete(entity);

            return rowsAffected;
        }

        protected override void ClearCache()
        {
            base.ClearCache();
            CacheManager.RemoveByPattern("Repository_ContentBlocks_ByPageId_.*");
            CacheManager.RemoveByPattern("Repository_ContentBlocks_ByPageIdAndZoneAndIncDisabled_.*");
        }

        #endregion GenericDataService<ContentBlock> Overrides

        #region IContentBlockService Members

        public IEnumerable<IContentBlock> GetContentBlocks(Guid pageId, string cultureCode)
        {
            string key = string.Format(
                "Repository_ContentBlocks_ByPageId_{0}_{1}",
                pageId,
                cultureCode);

            var records = CacheManager.Get(key, () =>
            {
                return Find(x => x.PageId == pageId);
            });

            return GetContentBlocks(records, cultureCode);
        }

        public IEnumerable<IContentBlock> GetContentBlocks(string zoneName, string cultureCode, Guid? pageId = null, bool includeDisabled = false)
        {
            int tenantId = GetTenantId();

            string key = string.Format(
                "Repository_ContentBlocks_ByPageIdAndZoneAndIncDisabled_{0}_{1}_{2}_{3}_{4}",
                tenantId,
                pageId,
                cultureCode,
                zoneName,
                includeDisabled);

            var records = Enumerable.Empty<ContentBlock>();

            if (includeDisabled)
            {
                records = CacheManager.Get(key, () =>
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
                        return Enumerable.Empty<ContentBlock>();
                    }

                    return pageId.HasValue
                        ? Find(x => x.ZoneId == zone.Id && x.PageId == pageId.Value)
                        : Find(x => x.ZoneId == zone.Id && x.PageId == null);
                });
            }
            else
            {
                records = CacheManager.Get(key, () =>
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
                        return Enumerable.Empty<ContentBlock>();
                    }

                    var list = Find(x => x.IsEnabled && x.ZoneId == zone.Id && x.PageId == null).ToList();

                    if (pageId.HasValue)
                    {
                        list.AddRange(Find(x => x.IsEnabled && x.ZoneId == zone.Id && x.PageId == pageId.Value).ToList());
                    }

                    return list;
                });
            }

            return GetContentBlocks(records, cultureCode);
        }

        #endregion IContentBlockService Members

        private IEnumerable<IContentBlock> GetContentBlocks(IEnumerable<ContentBlock> records, string cultureCode)
        {
            string entityType = typeof(ContentBlock).FullName;
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

                    if (record.BlockValues == null)
                    {
                        // Prevent error when trying to deserialize NULL value
                        record.BlockValues = "{}";
                    }

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
                contentBlock.PageId = record.PageId;
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