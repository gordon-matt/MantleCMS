using Autofac;
using Dependo;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Plugins.Widgets.Bootstrap3.ContentBlocks;
using Mantle.Web.Configuration;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Plugins.Widgets.Bootstrap3.Infrastructure
{
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
            builder.Register<IContentBlock, Bootstrap3CarouselBlock>(ServiceLifetime.Transient);
            builder.Register<IContentBlock, Bootstrap3ImageGalleryBlock>(ServiceLifetime.Transient);
            builder.Register<ISettings, Bootstrap3PluginSettings>(ServiceLifetime.Scoped);
        }

        public int Order => 9999;

        #endregion IDependencyRegistrar Members
    }
}