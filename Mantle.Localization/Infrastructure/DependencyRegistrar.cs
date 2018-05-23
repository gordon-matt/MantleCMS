using Autofac;
using Mantle.Infrastructure;
using Mantle.Localization.Services;

namespace Mantle.Localization.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<LanguageService>().As<ILanguageService>().InstancePerDependency();
            builder.RegisterType<LocalizableStringService>().As<ILocalizableStringService>().InstancePerDependency();
            builder.RegisterType<LocalizablePropertyService>().As<ILocalizablePropertyService>().InstancePerDependency();
        }

        public int Order => 0;

        #endregion IDependencyRegistrar Members
    }
}