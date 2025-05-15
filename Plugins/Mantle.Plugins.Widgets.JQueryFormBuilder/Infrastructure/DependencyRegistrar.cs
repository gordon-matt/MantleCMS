using Dependo;
using Mantle.Localization;
using Mantle.Plugins.Widgets.JQueryFormBuilder.ContentBlocks;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Plugins.Widgets.JQueryFormBuilder.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(IContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
        {
            return;
        }

        builder.Register<ILanguagePack, LanguagePackInvariant>(ServiceLifetime.Singleton);
        builder.Register<IContentBlock, JQueryFormBuilderBlock>(ServiceLifetime.Transient);
    }

    public int Order => 9999;

    #endregion IDependencyRegistrar Members
}