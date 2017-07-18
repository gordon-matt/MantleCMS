using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Mantle.Caching;
using Mantle.Infrastructure;
using Mantle.Logging;
using Microsoft.Extensions.Logging;

namespace Mantle.Data.Services
{
    public class GenericDataService<TEntity> : IGenericDataService<TEntity> where TEntity : class
    {
        #region Private Members

        private static string cacheKey;
        private static string cacheKeyFiltered;
        private readonly ICacheManager cacheManager;
        private readonly IRepository<TEntity> repository;

        #endregion Private Members

        #region Properties

        protected virtual string CacheKey
        {
            get
            {
                if (string.IsNullOrEmpty(cacheKey))
                {
                    cacheKey = string.Format("Repository_{0}", typeof(TEntity).Name);
                }
                return cacheKey;
            }
        }

        protected virtual string CacheKeyFiltered
        {
            get
            {
                if (string.IsNullOrEmpty(cacheKeyFiltered))
                {
                    cacheKeyFiltered = string.Format("Repository_{0}_{{0}}", typeof(TEntity).Name);
                }
                return cacheKeyFiltered;
            }
        }

        public ICacheManager CacheManager
        {
            get { return cacheManager; }
        }

        public ILogger Logger { get; private set; }

        #endregion Properties

        #region Constructor

        public GenericDataService(
            ICacheManager cacheManager, 
            IRepository<TEntity> repository)
        {
            this.cacheManager = cacheManager;
            this.repository = repository;

            var loggerFactory = EngineContext.Current.Resolve<ILoggerFactory>();
            Logger = loggerFactory.CreateLogger<GenericDataService<TEntity>>();
        }

        #endregion Constructor

        #region IGenericDataService<TEntity> Members

        #region Find

        public virtual IEnumerable<TEntity> Find(params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            return CacheManager.Get(CacheKey, () =>
            {
                return repository.Find(includePaths);
            });
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filterExpression, params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            return repository.Find(filterExpression, includePaths);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            return await CacheManager.Get(CacheKey, async () =>
            {
                return await repository.FindAsync(includePaths);
            });
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filterExpression, params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            return await repository.FindAsync(filterExpression, includePaths);
        }

        public virtual TEntity FindOne(params object[] keyValues)
        {
            return repository.FindOne(keyValues);
        }

        public virtual TEntity FindOne(Expression<Func<TEntity, bool>> filterExpression, params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            return repository.FindOne(filterExpression, includePaths);
        }

        public virtual async Task<TEntity> FindOneAsync(params object[] keyValues)
        {
            return await repository.FindOneAsync(keyValues);
        }

        public virtual async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression, params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            return await repository.FindOneAsync(filterExpression, includePaths);
        }

        #endregion Find

        #region Open/Use Connection

        public virtual IRepositoryConnection<TEntity> OpenConnection()
        {
            return repository.OpenConnection();
        }

        public virtual IRepositoryConnection<TEntity> UseConnection<TOther>(IRepositoryConnection<TOther> connection)
            where TOther : class
        {
            return repository.UseConnection(connection);
        }

        #endregion Open/Use Connection

        #region Count

        public virtual int Count()
        {
            return repository.Count();
        }

        public virtual int Count(Expression<Func<TEntity, bool>> countExpression)
        {
            return repository.Count(countExpression);
        }

        public virtual async Task<int> CountAsync()
        {
            return await repository.CountAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> countExpression)
        {
            return await repository.CountAsync(countExpression);
        }

        #endregion Count

        #region Delete

        public virtual int DeleteAll()
        {
            int rowsAffected = repository.DeleteAll();
            ClearCache();
            return rowsAffected;
        }

        public virtual int Delete(TEntity entity)
        {
            int rowsAffected = repository.Delete(entity);
            ClearCache();
            return rowsAffected;
        }

        public virtual int Delete(IEnumerable<TEntity> entities)
        {
            int rowsAffected = repository.Delete(entities);
            ClearCache();
            return rowsAffected;
        }

        public virtual int Delete(Expression<Func<TEntity, bool>> filterExpression)
        {
            int rowsAffected = repository.Delete(filterExpression);
            ClearCache();
            return rowsAffected;
        }

        public virtual int Delete(IQueryable<TEntity> query)
        {
            int rowsAffected = repository.Delete(query);
            ClearCache();
            return rowsAffected;
        }

        public virtual async Task<int> DeleteAllAsync()
        {
            return await repository.DeleteAllAsync();
        }

        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            return await repository.DeleteAsync(entity);
        }

        public virtual async Task<int> DeleteAsync(IEnumerable<TEntity> entities)
        {
            return await repository.DeleteAsync(entities);
        }

        public virtual async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return await repository.DeleteAsync(filterExpression);
        }

        public virtual async Task<int> DeleteAsync(IQueryable<TEntity> query)
        {
            return await repository.DeleteAsync(query);
        }

        #endregion Delete

        #region Insert

        public virtual int Insert(TEntity entity)
        {
            int rowsAffected = repository.Insert(entity);
            ClearCache();
            return rowsAffected;
        }

        public virtual int Insert(IEnumerable<TEntity> entities)
        {
            int rowsAffected = repository.Insert(entities);
            ClearCache();
            return rowsAffected;
        }

        public virtual async Task<int> InsertAsync(TEntity entity)
        {
            int rowsAffected = await repository.InsertAsync(entity);
            ClearCache();
            return rowsAffected;
        }

        public virtual async Task<int> InsertAsync(IEnumerable<TEntity> entities)
        {
            int rowsAffected = await repository.InsertAsync(entities);
            ClearCache();
            return rowsAffected;
        }

        #endregion Insert

        #region Update

        public virtual int Update(TEntity entity)
        {
            int rowsAffected = repository.Update(entity);
            ClearCache();
            return rowsAffected;
        }

        public virtual int Update(IEnumerable<TEntity> entities)
        {
            int rowsAffected = repository.Update(entities);
            ClearCache();
            return rowsAffected;
        }

        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            int rowsAffected = await repository.UpdateAsync(entity);
            ClearCache();
            return rowsAffected;
        }

        public virtual async Task<int> UpdateAsync(IEnumerable<TEntity> entities)
        {
            int rowsAffected = await repository.UpdateAsync(entities);
            ClearCache();
            return rowsAffected;
        }

        #endregion Update

        #endregion IGenericDataService<TEntity> Members

        protected virtual void ClearCache()
        {
            CacheManager.Remove(CacheKey);
            CacheManager.RemoveByPattern(string.Format(CacheKeyFiltered, ".*"));
        }
    }
}