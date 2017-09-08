using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Mantle.Collections;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Exceptions;
using Mantle.Infrastructure;
using Mantle.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mantle.Data.Entity.EntityFramework
{
    public class EntityFrameworkRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        #region Private Members

        private IDbContextFactory contextFactory;
        private readonly ILogger logger;

        #endregion Private Members

        #region Constructor

        public EntityFrameworkRepository(
            ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<EntityFrameworkRepository<TEntity>>();
        }

        #endregion Constructor

        #region IRepository<TEntity> Members

        public IRepositoryConnection<TEntity> OpenConnection()
        {
            var context = contextFactory.GetContext();
            return new EntityFrameworkRepositoryConnection<TEntity>(context, true);
        }

        public IRepositoryConnection<TEntity> UseConnection<TOther>(IRepositoryConnection<TOther> connection)
            where TOther : class
        {
            if (!(connection is EntityFrameworkRepositoryConnection<TOther>))
            {
                throw new NotSupportedException("The other connection must be of type EntityFrameworkRepositoryConnection<T>");
            }

            var otherConnection = (connection as EntityFrameworkRepositoryConnection<TOther>);
            return new EntityFrameworkRepositoryConnection<TEntity>(otherConnection.Context, false);
        }

        #region Find

        public IEnumerable<TEntity> Find(params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            using (var context = contextFactory.GetContext())
            {
                IQueryable<TEntity> query = context.Set<TEntity>().AsNoTracking();

                foreach (var path in includePaths)
                {
                    query = query.Include(path);
                }

                return query.ToHashSet();
            }
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            using (var context = contextFactory.GetContext())
            {
                var query = context.Set<TEntity>().AsNoTracking().Where(predicate);

                foreach (var path in includePaths)
                {
                    query = query.Include(path);
                }

                return query.ToHashSet();
            }
        }

        public async Task<IEnumerable<TEntity>> FindAsync(params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            using (var context = contextFactory.GetContext())
            {
                IQueryable<TEntity> query = context.Set<TEntity>().AsNoTracking();

                foreach (var path in includePaths)
                {
                    query = query.Include(path);
                }

                return await query.ToListAsync();
            }
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            using (var context = contextFactory.GetContext())
            {
                var query = context.Set<TEntity>().AsNoTracking().Where(predicate);

                foreach (var path in includePaths)
                {
                    query = query.Include(path);
                }

                return await query.ToListAsync();
            }
        }

        public TEntity FindOne(params object[] keyValues)
        {
            using (var context = contextFactory.GetContext())
            {
                return context.Set<TEntity>().Find(keyValues);
            }
        }

        public TEntity FindOne(object[] keyValues, params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            using (var context = contextFactory.GetContext())
            {
                var entity = context.Set<TEntity>().Find(keyValues);

                foreach (var path in includePaths)
                {
                    if (path.Body.Type.IsCollection())
                    {
                        context.Entry(entity).Collection((path.Body as MemberExpression).Member.Name).Load();
                    }
                    else
                    {
                        context.Entry(entity).Reference(path).Load();
                    }
                }

                return entity;
            }
        }

        public TEntity FindOne(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            using (var context = contextFactory.GetContext())
            {
                var query = context.Set<TEntity>().AsNoTracking().Where(predicate);

                foreach (var path in includePaths)
                {
                    query = query.Include(path);
                }

                return query.FirstOrDefault();
            }
        }

        public async Task<TEntity> FindOneAsync(params object[] keyValues)
        {
            using (var context = contextFactory.GetContext())
            {
                return await context.Set<TEntity>().FindAsync(keyValues);
            }
        }

        public async Task<TEntity> FindOneAsync(object[] keyValues, params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            using (var context = contextFactory.GetContext())
            {
                var entity = await context.Set<TEntity>().FindAsync(keyValues);

                foreach (var path in includePaths)
                {
                    if (path.Body.Type.IsCollection())
                    {
                        await context.Entry(entity).Collection((path.Body as MemberExpression).Member.Name).LoadAsync();
                    }
                    else
                    {
                        await context.Entry(entity).Reference(path).LoadAsync();
                    }
                }

                return entity;
            }
        }

        public async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, dynamic>>[] includePaths)
        {
            using (var context = contextFactory.GetContext())
            {
                var query = context.Set<TEntity>().AsNoTracking().Where(predicate);

                foreach (var path in includePaths)
                {
                    query = query.Include(path);
                }

                return await query.FirstOrDefaultAsync();
            }
        }

        #endregion Find

        #region Count

        public int Count()
        {
            using (var context = contextFactory.GetContext())
            {
                return context.Set<TEntity>().AsNoTracking().Count();
            }
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = contextFactory.GetContext())
            {
                return context.Set<TEntity>().AsNoTracking().Count(predicate);
            }
        }

        public async Task<int> CountAsync()
        {
            using (var context = contextFactory.GetContext())
            {
                return await context.Set<TEntity>().AsNoTracking().CountAsync();
            }
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = GetContext())
            {
                return await context.Set<TEntity>().AsNoTracking().CountAsync(predicate);
            }
        }

        #endregion Count

        #region Delete

        public int DeleteAll()
        {
            using (var context = GetContext())
            {
                var set = context.Set<TEntity>().AsNoTracking();
                // TODO: This will cause out-of-memory exceptions with tables that have too many records. We need a better solution!
                //  Change this to use a while loop and use Skip() and Take() to get paged results to delete.
                var entities = set.ToHashSet();
                return Delete(entities);
            }
        }

        public int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = GetContext())
            {
                var set = context.Set<TEntity>();
                // TODO: This will cause out-of-memory exceptions with tables that have too many records. We need a better solution!
                //  Change this to use a while loop and use Skip() and Take() to get paged results to delete.
                var entities = set.Where(predicate).ToHashSet();
                return Delete(entities);
            }
        }

        public int Delete(TEntity entity)
        {
            using (var context = GetContext())
            {
                var set = context.Set<TEntity>();

                if (context.Entry(entity).State == EntityState.Detached)
                {
                    var localEntity = set.Local.FirstOrDefault(x => x.KeyValues.ArrayEquals(entity.KeyValues));

                    if (localEntity != null)
                    {
                        context.Entry(localEntity).State = EntityState.Deleted;
                    }
                    else
                    {
                        set.Attach(entity);
                        context.Entry(entity).State = EntityState.Deleted;
                    }
                }
                else
                {
                    set.Remove(entity);
                }
                return context.SaveChanges();
            }
        }

        public int Delete(IEnumerable<TEntity> entities)
        {
            using (var context = GetContext())
            {
                var set = context.Set<TEntity>();

                foreach (var entity in entities)
                {
                    if (context.Entry(entity).State == EntityState.Detached)
                    {
                        var localEntity = set.Local.FirstOrDefault(x => x.KeyValues.ArrayEquals(entity.KeyValues));

                        if (localEntity != null)
                        {
                            context.Entry(localEntity).State = EntityState.Deleted;
                        }
                        else
                        {
                            set.Attach(entity);
                            context.Entry(entity).State = EntityState.Deleted;
                        }
                    }
                    else
                    {
                        set.Remove(entity);
                    }
                }
                return context.SaveChanges();
            }
        }

        public async Task<int> DeleteAllAsync()
        {
            using (var context = GetContext())
            {
                var set = context.Set<TEntity>().AsNoTracking();
                // TODO: This will cause out-of-memory exceptions with tables that have too many records. We need a better solution!
                //  Change this to use a while loop and use Skip() and Take() to get paged results to delete.
                var entities = set.ToHashSet();
                return await DeleteAsync(entities);
            }
        }

        public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = GetContext())
            {
                var set = context.Set<TEntity>().AsNoTracking();
                // TODO: This will cause out-of-memory exceptions with tables that have too many records. We need a better solution!
                //  Change this to use a while loop and use Skip() and Take() to get paged results to delete.
                var entities = set.Where(predicate).ToHashSet();
                return await DeleteAsync(entities);
            }
        }

        public async Task<int> DeleteAsync(TEntity entity)
        {
            using (var context = GetContext())
            {
                var set = context.Set<TEntity>();

                if (context.Entry(entity).State == EntityState.Detached)
                {
                    var localEntity = set.Local.FirstOrDefault(x => x.KeyValues.ArrayEquals(entity.KeyValues));

                    if (localEntity != null)
                    {
                        context.Entry(localEntity).State = EntityState.Deleted;
                    }
                    else
                    {
                        set.Attach(entity);
                        context.Entry(entity).State = EntityState.Deleted;
                    }
                }
                else
                {
                    set.Remove(entity);
                }
                return await context.SaveChangesAsync();
            }
        }

        public async Task<int> DeleteAsync(IEnumerable<TEntity> entities)
        {
            using (var context = GetContext())
            {
                var set = context.Set<TEntity>();

                foreach (var entity in entities)
                {
                    if (context.Entry(entity).State == EntityState.Detached)
                    {
                        var localEntity = set.Local.FirstOrDefault(x => x.KeyValues.ArrayEquals(entity.KeyValues));

                        if (localEntity != null)
                        {
                            context.Entry(localEntity).State = EntityState.Deleted;
                        }
                        else
                        {
                            set.Attach(entity);
                            context.Entry(entity).State = EntityState.Deleted;
                        }
                    }
                    else
                    {
                        set.Remove(entity);
                    }
                }
                return await context.SaveChangesAsync();
            }
        }

        #endregion Delete

        #region Insert

        public int Insert(TEntity entity)
        {
            using (var context = GetContext())
            {
                context.Set<TEntity>().Add(entity);
                return context.SaveChanges();
            }
        }

        public int Insert(IEnumerable<TEntity> entities)
        {
            using (var context = GetContext())
            {
                foreach (var entity in entities)
                {
                    context.Set<TEntity>().Add(entity);
                }
                return context.SaveChanges();
            }
        }

        public async Task<int> InsertAsync(TEntity entity)
        {
            using (var context = GetContext())
            {
                context.Set<TEntity>().Add(entity);
                return await context.SaveChangesAsync();
            }
        }

        public async Task<int> InsertAsync(IEnumerable<TEntity> entities)
        {
            using (var context = GetContext())
            {
                int count = entities.Count();
                foreach (var entity in entities)
                {
                    context.Set<TEntity>().Add(entity);
                }
                return await context.SaveChangesAsync();
            }
        }

        #endregion Insert

        #region Update

        public int Update(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }

                using (var context = GetContext())
                {
                    var set = context.Set<TEntity>();

                    if (context.Entry(entity).State == EntityState.Detached)
                    {
                        var localEntity = set.Local.FirstOrDefault(x => x.KeyValues.ArrayEquals(entity.KeyValues));

                        if (localEntity != null)
                        {
                            context.Entry(localEntity).CurrentValues.SetValues(entity);
                            context.Entry(localEntity).State = EntityState.Modified;
                        }
                        else
                        {
                            entity = set.Attach(entity).Entity;
                            context.Entry(entity).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        context.Entry(entity).State = EntityState.Modified;
                    }

                    return context.SaveChanges();
                }
            }
            catch (Exception x)
            {
                string message = x.GetBaseException().Message;
                logger.LogError(new EventId(), x, message);
                throw new MantleException(message);
            }
        }

        public int Update(IEnumerable<TEntity> entities)
        {
            try
            {
                if (entities == null)
                {
                    throw new ArgumentNullException(nameof(entities));
                }

                using (var context = GetContext())
                {
                    var set = context.Set<TEntity>();

                    foreach (var entity in entities)
                    {
                        if (context.Entry(entity).State == EntityState.Detached)
                        {
                            var localEntity = set.Local.FirstOrDefault(x => x.KeyValues.ArrayEquals(entity.KeyValues));

                            if (localEntity != null)
                            {
                                context.Entry(localEntity).CurrentValues.SetValues(entity);
                            }
                            else
                            {
                                set.Attach(entity);
                                context.Entry(entity).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            context.Entry(entity).State = EntityState.Modified;
                        }
                    }
                    return context.SaveChanges();
                }
            }
            catch (Exception x)
            {
                string message = x.GetBaseException().Message;
                logger.LogError(new EventId(), x, message);
                throw new MantleException(message);
            }
        }

        public async Task<int> UpdateAsync(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }

                using (var context = GetContext())
                {
                    var set = context.Set<TEntity>();

                    if (context.Entry(entity).State == EntityState.Detached)
                    {
                        var localEntity = set.Local.FirstOrDefault(x => x.KeyValues.ArrayEquals(entity.KeyValues));

                        if (localEntity != null)
                        {
                            context.Entry(localEntity).CurrentValues.SetValues(entity);
                            context.Entry(localEntity).State = EntityState.Modified;
                        }
                        else
                        {
                            entity = set.Attach(entity).Entity;
                            context.Entry(entity).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        context.Entry(entity).State = EntityState.Modified;
                    }

                    return await context.SaveChangesAsync();
                }
            }
            catch (Exception x)
            {
                string message = x.GetBaseException().Message;
                logger.LogError(new EventId(), x, message);
                throw new MantleException(message);
            }
        }

        public async Task<int> UpdateAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                if (entities == null)
                {
                    throw new ArgumentNullException(nameof(entities));
                }

                using (var context = GetContext())
                {
                    var set = context.Set<TEntity>();

                    foreach (var entity in entities)
                    {
                        if (context.Entry(entity).State == EntityState.Detached)
                        {
                            var localEntity = set.Local.FirstOrDefault(x => x.KeyValues.ArrayEquals(entity.KeyValues));

                            if (localEntity != null)
                            {
                                context.Entry(localEntity).CurrentValues.SetValues(entity);
                            }
                            else
                            {
                                set.Attach(entity);
                                context.Entry(entity).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            context.Entry(entity).State = EntityState.Modified;
                        }
                    }
                    return await context.SaveChangesAsync();
                }
            }
            catch (Exception x)
            {
                string message = x.GetBaseException().Message;
                logger.LogError(new EventId(), x, message);
                throw new MantleException(message);
            }
        }

        #endregion Update

        #endregion IRepository<TEntity> Members

        protected virtual DbContext GetContext()
        {
            if (contextFactory == null)
            {
                contextFactory = EngineContext.Current.Resolve<IDbContextFactory>();
            }
            return contextFactory.GetContext();
        }
    }
}