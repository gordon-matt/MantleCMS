using Autofac;
using Mantle.Infrastructure;
using Mantle.Web.Infrastructure;

namespace Mantle.Web.CommonResources.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            // Embedded File Provider
            builder.RegisterType<EmbeddedFileProviderRegistrar>().As<IEmbeddedFileProviderRegistrar>().InstancePerLifetimeScope();
        }

        public int Order => 1;

        #endregion IDependencyRegistrar Members
    }
}