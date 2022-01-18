using Autofac;
using Mantle.Caching;
using Mantle.Infrastructure;
using Mantle.Web.Configuration;

namespace Mantle.Plugins.Caching.Redis.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
            {
                return;
            }

            builder.RegisterType<RedisCacheManager>().As<ICacheManager>().Named<ICacheManager>("Mantle_Cache_Static").SingleInstance();
            builder.RegisterType<RedisCacheManager>().As<ICacheManager>().Named<ICacheManager>("Mantle_Cache_Per_Request").InstancePerLifetimeScope();
            builder.RegisterType<RedisCacheSettings>().As<ISettings>().InstancePerLifetimeScope();
        }

        public int Order => 99999;
    }
}