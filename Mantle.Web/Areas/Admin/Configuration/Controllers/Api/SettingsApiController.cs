﻿namespace Mantle.Web.Areas.Admin.Configuration.Controllers.Api;

public class SettingsApiController : GenericTenantODataController<Setting, Guid>
{
    private readonly ICacheManager cacheManager;

    public SettingsApiController(IRepository<Setting> repository, ICacheManager cacheManager)
        : base(repository)
    {
        this.cacheManager = cacheManager;
    }

    protected override Guid GetId(Setting entity) => entity.Id;

    protected override void SetNewId(Setting entity) => entity.Id = Guid.NewGuid();

    public override async Task<IActionResult> Put([FromODataUri] Guid key, [FromBody] Setting entity)
    {
        var result = await base.Put(key, entity);

        string cacheKey = string.Format(MantleWebConstants.CacheKeys.SettingsKeyFormat, entity.TenantId, entity.Type);
        cacheManager.Remove(cacheKey);

        // TODO: This is an ugly hack. We need to have a way for each setting to perform some tasks after update
        if (entity.Name == new SiteSettings().Name)
        {
            cacheManager.Remove(MantleWebConstants.CacheKeys.CurrentCulture);
        }

        return result;
    }

    public override async Task<IActionResult> Post([FromBody] Setting entity)
    {
        var result = await base.Post(entity);

        string cacheKey = string.Format(MantleWebConstants.CacheKeys.SettingsKeyFormat, entity.TenantId, entity.Type);
        cacheManager.Remove(cacheKey);

        return result;
    }

    public override async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<Setting> patch)
    {
        var result = await base.Patch(key, patch);

        var entity = await Service.FindOneAsync(key);
        string cacheKey = string.Format(MantleWebConstants.CacheKeys.SettingsKeyFormat, entity.TenantId, entity.Type);
        cacheManager.Remove(cacheKey);

        return result;
    }

    public override async Task<IActionResult> Delete([FromODataUri] Guid key)
    {
        var result = base.Delete(key);

        var entity = await Service.FindOneAsync(key);
        string cacheKey = string.Format(MantleWebConstants.CacheKeys.SettingsKeyFormat, entity.TenantId, entity.Type);
        cacheManager.Remove(cacheKey);

        return await result;
    }

    protected override Permission ReadPermission => MantleWebPermissions.SettingsRead;

    protected override Permission WritePermission => MantleWebPermissions.SettingsWrite;
}