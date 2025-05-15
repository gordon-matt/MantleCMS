using Mantle.Tenants.Entities;

namespace Mantle.Web.OData;

public abstract class GenericTenantODataController<TEntity, TKey> : GenericODataController<TEntity, TKey>
    where TEntity : class, ITenantEntity
{
    private readonly IWorkContext workContext;

    #region Constructors

    public GenericTenantODataController(IGenericDataService<TEntity> service)
        : base(service)
    {
        this.workContext = DependoResolver.Instance.Resolve<IWorkContext>();
    }

    public GenericTenantODataController(IRepository<TEntity> repository)
        : base(repository)
    {
        this.workContext = DependoResolver.Instance.Resolve<IWorkContext>();
    }

    #endregion Constructors

    #region GenericODataController<TEntity, TKey> Members

    protected override IQueryable<TEntity> ApplyMandatoryFilter(IQueryable<TEntity> query)
    {
        int tenantId = GetTenantId();
        if (CheckPermission(StandardPermissions.FullAccess))
        {
            // TODO: Not sure if this is the best solution. Maybe we should only show the items with NULL for Tenant ID?
            return query.Where(x => x.TenantId == null || x.TenantId == tenantId);
        }
        return query.Where(x => x.TenantId == tenantId);
    }

    #endregion GenericODataController<TEntity, TKey> Members

    protected virtual int GetTenantId() => workContext.CurrentTenant.Id;

    protected override bool CanViewEntity(TEntity entity)
    {
        if (entity == null)
        {
            return false;
        }

        if (CheckPermission(StandardPermissions.FullAccess))
        {
            return true; // Only the super admin should have full access
        }

        // If not admin user, but possibly the tenant user...

        if (CheckPermission(ReadPermission))
        {
            int tenantId = GetTenantId();
            return entity.TenantId == tenantId;
        }

        return false;
    }

    protected override bool CanModifyEntity(TEntity entity)
    {
        if (entity == null)
        {
            return false;
        }

        if (CheckPermission(StandardPermissions.FullAccess))
        {
            return true; // Only the super admin should have full access
        }

        // If not admin user, but possibly the tenant...

        if (CheckPermission(WritePermission))
        {
            int tenantId = GetTenantId();
            return entity.TenantId == tenantId;
        }

        return false;
    }

    protected override void OnBeforeSave(TEntity entity)
    {
        base.OnBeforeSave(entity);
        entity.TenantId = GetTenantId();
    }
}