using Extenso.Collections.Generic;

namespace Mantle.Data.Services;

public interface IGenericDataService<TEntity> where TEntity : class
{
    #region Open Connection

    /// <summary>
    /// Used to access an IQueryable and run custom queries directly against the database
    /// </summary>
    /// <returns></returns>
    IRepositoryConnection<TEntity> OpenConnection();

    IRepositoryConnection<TEntity> UseConnection<TOther>(IRepositoryConnection<TOther> connection)
        where TOther : class;

    #endregion Open Connection

    #region Find

    IPagedCollection<TEntity> Find(SearchOptions<TEntity> options);

    IPagedCollection<TResult> Find<TResult>(SearchOptions<TEntity> options, Expression<Func<TEntity, TResult>> projection);

    Task<IPagedCollection<TEntity>> FindAsync(SearchOptions<TEntity> options);

    Task<IPagedCollection<TResult>> FindAsync<TResult>(SearchOptions<TEntity> options, Expression<Func<TEntity, TResult>> projection);

    TEntity FindOne(params object[] keyValues);

    TEntity FindOne(SearchOptions<TEntity> options);

    TResult FindOne<TResult>(SearchOptions<TEntity> options, Expression<Func<TEntity, TResult>> projection);

    Task<TEntity> FindOneAsync(params object[] keyValues);

    Task<TEntity> FindOneAsync(SearchOptions<TEntity> options);

    Task<TResult> FindOneAsync<TResult>(SearchOptions<TEntity> options, Expression<Func<TEntity, TResult>> projection);

    #endregion Find

    #region Count

    int Count();

    int Count(Expression<Func<TEntity, bool>> countExpression);

    Task<int> CountAsync();

    Task<int> CountAsync(Expression<Func<TEntity, bool>> countExpression);

    #endregion Count

    #region Delete

    int DeleteAll();

    int Delete(TEntity entity);

    int Delete(IEnumerable<TEntity> entities);

    int Delete(Expression<Func<TEntity, bool>> filterExpression);

    int Delete(IQueryable<TEntity> query);

    Task<int> DeleteAllAsync();

    Task<int> DeleteAsync(TEntity entity);

    Task<int> DeleteAsync(IEnumerable<TEntity> entities);

    Task<int> DeleteAsync(Expression<Func<TEntity, bool>> filterExpression);

    Task<int> DeleteAsync(IQueryable<TEntity> query);

    #endregion Delete

    #region Insert

    int Insert(TEntity entity);

    int Insert(IEnumerable<TEntity> entities);

    Task<int> InsertAsync(TEntity entity);

    Task<int> InsertAsync(IEnumerable<TEntity> entities);

    #endregion Insert

    #region Update

    int Update(TEntity entity);

    int Update(IEnumerable<TEntity> entities);

    Task<int> UpdateAsync(TEntity entity);

    Task<int> UpdateAsync(IEnumerable<TEntity> entities);

    //int Update(Expression<Func<TEntity, TEntity>> updateExpression);

    //int Update(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TEntity>> updateExpression);

    //int Update(IQueryable<TEntity> query, Expression<Func<TEntity, TEntity>> updateExpression);

    #endregion Update
}