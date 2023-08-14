using Autofac;
using Mantle.Caching;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Web.Configuration;
using Microsoft.Extensions.Configuration;

namespace Mantle.Plugins.Caching.Redis.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
        {
            if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
            {
                return;
            }

            builder.RegisterType<RedisCacheSettings>().As<ISettings>().InstancePerLifetimeScope();
            builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();
            builder.RegisterType<RedisCacheManager>().As<ICacheManager>().Named<ICacheManager>("Mantle_Cache_Static").SingleInstance();
            builder.RegisterType<RedisCacheManager>().As<ICacheManager>().Named<ICacheManager>("Mantle_Cache_Per_Request").InstancePerLifetimeScope();
        }

        public int Order => 99999;
    }
}