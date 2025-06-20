﻿//using System.Linq;
//using Extenso.Data.Entity;
//using Mantle.Data.Services;
//using Mantle.Infrastructure;
//using Mantle.Security.Membership.Permissions;
//using Mantle.Tenants.Entities;

//namespace Mantle.Web.Mvc
//{
//    public abstract class MantleGenericTenantDataController<TEntity, TKey> : MantleGenericDataController<TEntity, TKey>
//        where TEntity : class, ITenantEntity
//    {
//        private readonly IWorkContext workContext;

//        #region Constructors

//        public MantleGenericTenantDataController(IRepository<TEntity> repository)
//            : base(repository)
//        {
//            workContext = DependoResolver.Instance.Resolve<IWorkContext>();
//        }

//        public MantleGenericTenantDataController(IGenericDataService<TEntity> service)
//            : base(service)
//        {
//            workContext = DependoResolver.Instance.Resolve<IWorkContext>();
//        }

//        #endregion Constructors

//        #region GenericODataController<TEntity, TKey> Members

//        protected override IQueryable<TEntity> ApplyMandatoryFilter(IQueryable<TEntity> query)
//        {
//            int tenantId = GetTenantId();
//            if (CheckPermission(StandardPermissions.FullAccess))
//            {
//                // TODO: Not sure if this is the best solution. Maybe we should only show the items with NULL for Tenant ID?
//                return query.Where(x => x.TenantId == null || x.TenantId == tenantId);
//            }
//            return query.Where(x => x.TenantId == tenantId);
//        }

//        #endregion GenericODataController<TEntity, TKey> Members

//        protected virtual int GetTenantId()
//        {
//            return workContext.CurrentTenant.Id;
//        }

//        protected override bool CanViewEntity(TEntity entity)
//        {
//            if (entity == null)
//            {
//                return false;
//            }

//            if (CheckPermission(StandardPermissions.FullAccess))
//            {
//                return true; // Only the super admin should have full access
//            }

//            // If not admin user, but possibly the tenant user...

//            if (CheckPermission(ReadPermission))
//            {
//                int tenantId = GetTenantId();
//                return entity.TenantId == tenantId;
//            }

//            return false;
//        }

//        protected override bool CanModifyEntity(TEntity entity)
//        {
//            if (entity == null)
//            {
//                return false;
//            }

//            if (CheckPermission(StandardPermissions.FullAccess))
//            {
//                return true; // Only the super admin should have full access
//            }

//            // If not admin user, but possibly the tenant...

//            if (CheckPermission(WritePermission))
//            {
//                int tenantId = GetTenantId();
//                return entity.TenantId == tenantId;
//            }

//            return false;
//        }

//        protected override void OnBeforeSave(TEntity entity)
//        {
//            base.OnBeforeSave(entity);
//            entity.TenantId = GetTenantId();
//        }
//    }
//}