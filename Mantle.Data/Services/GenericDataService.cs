using System.Linq.Expressions;
using Extenso.Collections.Generic;
using Mantle.Caching;
using Microsoft.Extensions.Logging;

namespace Mantle.Data.Services;

public class GenericDataService<TEntity> : IGenericDataService<TEntity> where TEntity : class
{
    #region Private Members

    private static string cacheKey;
    private static string cacheKeyFiltered;
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

    public ICacheManager CacheManager { get; private set; }

    public ILogger Logger { get; private set; }

    #endregion Properties

    #region Constructor

    public GenericDataService(
        ICacheManager cacheManager,
        IRepository<TEntity> repository)
    {
        CacheManager = cacheManager;
        this.repository = repository;

        var loggerFactory = DependoResolver.Instance.Resolve<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger<GenericDataService<TEntity>>();
    }

    #endregion Constructor

    #region IGenericDataService<TEntity> Members

    #region Find

    // TODO: Caching.

    public virtual IPagedCollection<TEntity> Find(SearchOptions<TEntity> options) =>
        repository.Find(options);

    public virtual IPagedCollection<TResult> Find<TResult>(SearchOptions<TEntity> options, Expression<Func<TEntity, TResult>> projection) =>
        repository.Find(options, projection);

    public virtual Task<IPagedCollection<TEntity>> FindAsync(SearchOptions<TEntity> options) =>
        repository.FindAsync(options);

    public virtual Task<IPagedCollection<TResult>> FindAsync<TResult>(SearchOptions<TEntity> options, Expression<Func<TEntity, TResult>> projection) =>
        repository.FindAsync(options, projection);

    public virtual TEntity FindOne(params object[] keyValues) =>
        repository.FindOne(keyValues);

    public virtual TEntity FindOne(SearchOptions<TEntity> options) =>
        repository.FindOne(options);

    public virtual TResult FindOne<TResult>(SearchOptions<TEntity> options, Expression<Func<TEntity, TResult>> projection) =>
        repository.FindOne(options, projection);

    public virtual Task<TEntity> FindOneAsync(params object[] keyValues) =>
        repository.FindOneAsync(keyValues);

    public virtual Task<TEntity> FindOneAsync(SearchOptions<TEntity> options) =>
        repository.FindOneAsync(options);

    public virtual Task<TResult> FindOneAsync<TResult>(SearchOptions<TEntity> options, Expression<Func<TEntity, TResult>> projection) =>
        repository.FindOneAsync(options, projection);

    #endregion Find

    #region Open/Use Connection

    public virtual IRepositoryConnection<TEntity> OpenConnection() => repository.OpenConnection();

    public virtual IRepositoryConnection<TEntity> UseConnection<TOther>(IRepositoryConnection<TOther> connection)
        where TOther : class => repository.UseConnection(connection);

    #endregion Open/Use Connection

    #region Count

    public virtual int Count() => repository.Count();

    public virtual int Count(Expression<Func<TEntity, bool>> countExpression) => repository.Count(countExpression);

    public virtual async Task<int> CountAsync() => await repository.CountAsync();

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> countExpression) => await repository.CountAsync(countExpression);

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

    public virtual async Task<int> DeleteAllAsync() => await repository.DeleteAllAsync();

    public virtual async Task<int> DeleteAsync(TEntity entity) => await repository.DeleteAsync(entity);

    public virtual async Task<int> DeleteAsync(IEnumerable<TEntity> entities) => await repository.DeleteAsync(entities);

    public virtual async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> filterExpression) => await repository.DeleteAsync(filterExpression);

    public virtual async Task<int> DeleteAsync(IQueryable<TEntity> query) => await repository.DeleteAsync(query);

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

    public virtual TEntity Update(TEntity entity)
    {
        entity = repository.Update(entity);
        ClearCache();
        return entity;
    }

    public virtual IEnumerable<TEntity> Update(IEnumerable<TEntity> entities)
    {
        entities = repository.Update(entities);
        ClearCache();
        return entities;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity = await repository.UpdateAsync(entity);
        ClearCache();
        return entity;
    }

    public virtual async Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities)
    {
        entities = await repository.UpdateAsync(entities);
        ClearCache();
        return entities;
    }

    #endregion Update

    #endregion IGenericDataService<TEntity> Members

    protected virtual void ClearCache()
    {
        CacheManager.Remove(CacheKey);
        CacheManager.RemoveByPattern(string.Format(CacheKeyFiltered, ".*"));
    }
}