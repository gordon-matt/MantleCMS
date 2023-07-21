using Extenso.Data.Entity;
using Extenso.Data.QueryBuilder;
using System.Data;

namespace Mantle.Data.Dapper
{
    public interface IDapperRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        IDbConnection OpenConnection();

        IEnumerable<TEntity> Find();

        IEnumerable<TEntity> Find(ISelectQueryBuilder queryBuilder);

        IEnumerable<TEntity> Find(string predicate, int skip = 0, short take = -1, string orderBy = null, SortDirection sortDirection = SortDirection.Ascending);

        TEntity FindOne(TKey id);

        TEntity FindOne(string predicate);

        int Count();

        int Count(ISelectQueryBuilder queryBuilder);

        int Count(string predicate);

        int Delete(TEntity entity);

        int Delete(IEnumerable<TEntity> entities);

        bool Insert(TEntity entity);

        int Insert(IEnumerable<TEntity> entities);

        int Update(TEntity entity);

        int Update(IEnumerable<TEntity> entities);

        ISelectQueryBuilder CreateQuery();
    }
}