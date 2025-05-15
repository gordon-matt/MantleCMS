using Dependo;
using Mantle.Localization;
using Mantle.Plugins.Widgets.FlexSlider.ContentBlocks;
using Mantle.Web.Configuration;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Mantle.Web.Mvc.Themes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Plugins.Widgets.FlexSlider.Infrastructure;

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
        builder.Register<IContentBlock, FlexSliderBlock>(ServiceLifetime.Transient);

        builder.Register<ILocationFormatProvider, LocationFormatProvider>(ServiceLifetime.Singleton);
        builder.Register<ISettings, FlexSliderPluginSettings>(ServiceLifetime.Scoped);
    }

    public int Order => 9999;

    #endregion IDependencyRegistrar Members
}