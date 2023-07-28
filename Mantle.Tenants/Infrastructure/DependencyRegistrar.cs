using Autofac;
using Mantle.Infrastructure;
using Mantle.Tenants.Services;

namespace Mantle.Tenants.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
    {
        builder.RegisterType<TenantService>().As<ITenantService>().InstancePerDependency();
    }

    public int Order => 0;

    #endregion IDependencyRegistrar Members
}