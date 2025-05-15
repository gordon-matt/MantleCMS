using Dependo;
using Extenso.AspNetCore.OData;
using Mantle.Localization;
using Mantle.Plugins.Messaging.Forums.Services;
using Mantle.Web.ContentManagement.Infrastructure;
using Mantle.Web.Infrastructure;
using Mantle.Web.Navigation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Plugins.Messaging.Forums.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(IContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
        {
            return;
        }

        builder.Register<IDurandalRouteProvider, DurandalRouteProvider>(ServiceLifetime.Singleton);

        // Embedded File Provider
        builder.Register<IEmbeddedFileProviderRegistrar, EmbeddedFileProviderRegistrar>(ServiceLifetime.Scoped);

        builder.Register<ISettings, ForumSettings>(ServiceLifetime.Scoped);
        builder.Register<ILanguagePack, LanguagePackInvariant>(ServiceLifetime.Singleton);

        builder.Register<IPermissionProvider, ForumPermissions>(ServiceLifetime.Singleton);
        builder.Register<ILocationFormatProvider, LocationFormatProvider>(ServiceLifetime.Singleton);
        builder.Register<INavigationProvider, NavigationProvider>(ServiceLifetime.Singleton);
        builder.Register<IODataRegistrar, ODataRegistrar>(ServiceLifetime.Singleton);

        builder.Register<IAutoMenuProvider, AutoMenuProvider>(ServiceLifetime.Singleton);
        builder.Register<IForumService, ForumService>(ServiceLifetime.Transient);
    }

    public int Order => 9999;

    #endregion IDependencyRegistrar Members
}