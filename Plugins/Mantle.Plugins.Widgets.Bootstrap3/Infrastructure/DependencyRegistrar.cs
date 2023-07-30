using Autofac;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Plugins.Widgets.Bootstrap3.ContentBlocks;
using Mantle.Web.Configuration;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Plugins.Widgets.Bootstrap3.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
            {
                return;
            }

            builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();
            builder.RegisterType<Bootstrap3CarouselBlock>().As<IContentBlock>().InstancePerDependency();
            builder.RegisterType<Bootstrap3ImageGalleryBlock>().As<IContentBlock>().InstancePerDependency();
            builder.RegisterType<Bootstrap3PluginSettings>().As<ISettings>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 9999; }
        }

        #endregion IDependencyRegistrar Members
    }
}