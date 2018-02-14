using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Mantle.Data.Services
{
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

        /// <summary>
        /// Retrieve all entities
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> Find(params Expression<Func<TEntity, dynamic>>[] includePaths);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filterExpression, params Expression<Func<TEntity, dynamic>>[] includePaths);

        Task<IEnumerable<TEntity>> FindAsync(params Expression<Func<TEntity, dynamic>>[] includePaths);

        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filterExpression, params Expression<Func<TEntity, dynamic>>[] includePaths);

        TEntity FindOne(params object[] keyValues);

        //TEntity FindOne(object[] keyValues, params Expression<Func<TEntity, dynamic>>[] includePaths);

        TEntity FindOne(Expression<Func<TEntity, bool>> filterExpression, params Expression<Func<TEntity, dynamic>>[] includePaths);

        Task<TEntity> FindOneAsync(params object[] keyValues);

        //Task<TEntity> FindOneAsync(object[] keyValues, params Expression<Func<TEntity, dynamic>>[] includePaths);

        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression, params Expression<Func<TEntity, dynamic>>[] includePaths);

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
}