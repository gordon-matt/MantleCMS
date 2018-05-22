using Extenso.Data.Entity;
using Mantle.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mantle.Data.Entity
{
    public class MantleEntityFrameworkRepository<TEntity> : EntityFrameworkRepository<TEntity>
        where TEntity : class, IEntity
    {
        private IDbContextFactory contextFactory;

        #region Constructor

        public MantleEntityFrameworkRepository(
            IDbContextFactory contextFactory,
            ILoggerFactory loggerFactory)
            : base(contextFactory, loggerFactory)
        {
            this.contextFactory = contextFactory;
        }

        #endregion Constructor

        protected override DbContext GetContext()
        {
            if (contextFactory == null)
            {
                contextFactory = EngineContext.Current.Resolve<IDbContextFactory>();
            }
            return contextFactory.GetContext();
        }
    }
}