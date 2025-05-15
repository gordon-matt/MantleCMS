using Microsoft.Extensions.Logging;

namespace Mantle.Data.Entity;

public class MantleEntityFrameworkRepository<TEntity> : EntityFrameworkRepository<TEntity>
    where TEntity : class, IEntity
{
    private new IDbContextFactory contextFactory;

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
        contextFactory ??= DependoResolver.Instance.Resolve<IDbContextFactory>();
        return contextFactory.GetContext();
    }
}