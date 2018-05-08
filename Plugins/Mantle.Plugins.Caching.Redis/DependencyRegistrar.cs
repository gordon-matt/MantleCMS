using Autofac;
using Mantle.Caching;
using Mantle.Infrastructure;
using Mantle.Plugins;

namespace Mantle.Plugins.Caching.Redis
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
            {
                return;
            }

            builder.RegisterType<RedisCacheManager>().As<ICacheManager>().Named<ICacheManager>("Mantle_Cache_Static").SingleInstance();
            builder.RegisterType<RedisCacheManager>().As<ICacheManager>().Named<ICacheManager>("Mantle_Cache_Per_Request").InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 99999; }
        }
    }
}