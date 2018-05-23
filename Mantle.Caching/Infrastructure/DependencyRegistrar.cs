using Autofac;
using Mantle.Infrastructure;
using Mantle.Tasks;

namespace Mantle.Caching.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("Mantle_Cache_Static").SingleInstance();
            builder.RegisterType<ClearCacheTask>().As<ITask>().SingleInstance();
        }

        public int Order => 0;

        #endregion IDependencyRegistrar Members
    }
}