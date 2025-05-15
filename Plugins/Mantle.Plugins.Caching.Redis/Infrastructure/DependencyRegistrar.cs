using Dependo;
using Mantle.Caching;
using Mantle.Localization;
using Mantle.Web.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Plugins.Caching.Redis.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    public void Register(IContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
        {
            return;
        }

        builder.Register<ISettings, RedisCacheSettings>(ServiceLifetime.Scoped);
        builder.Register<ILanguagePack, LanguagePackInvariant>(ServiceLifetime.Singleton);
        builder.RegisterNamed<ICacheManager, RedisCacheManager>("Mantle_Cache_Static", ServiceLifetime.Singleton);
        builder.RegisterNamed<ICacheManager, RedisCacheManager>("Mantle_Cache_Per_Request", ServiceLifetime.Scoped);
    }

    public int Order => 99999;
}