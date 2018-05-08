using Autofac;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks;

namespace Mantle.Plugins.Widgets.View360.Infrastructure
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
            builder.RegisterType<View360Block>().As<IContentBlock>().InstancePerDependency();
        }

        public int Order
        {
            get { return 9999; }
        }

        #endregion IDependencyRegistrar Members
    }
}