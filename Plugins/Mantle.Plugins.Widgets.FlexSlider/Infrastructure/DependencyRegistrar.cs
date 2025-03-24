using Autofac;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Plugins.Widgets.FlexSlider.ContentBlocks;
using Mantle.Web.Configuration;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Mantle.Web.Mvc.Themes;
using Microsoft.Extensions.Configuration;

namespace Mantle.Plugins.Widgets.FlexSlider.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
        {
            return;
        }

        builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();
        builder.RegisterType<FlexSliderBlock>().As<IContentBlock>().InstancePerDependency();

        builder.RegisterType<LocationFormatProvider>().As<ILocationFormatProvider>().SingleInstance();
        builder.RegisterType<FlexSliderPluginSettings>().As<ISettings>().InstancePerLifetimeScope();
    }

    public int Order => 9999;

    #endregion IDependencyRegistrar Members
}