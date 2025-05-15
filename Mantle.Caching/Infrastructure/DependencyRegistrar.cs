using Microsoft.Extensions.Configuration;

namespace Mantle.Caching.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(IContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        builder.RegisterNamed<ICacheManager, MemoryCacheManager>("Mantle_Cache_Static", ServiceLifetime.Singleton);
        builder.Register<ITask, ClearCacheTask>(ServiceLifetime.Singleton);
    }

    public int Order => 0;

    #endregion IDependencyRegistrar Members
}