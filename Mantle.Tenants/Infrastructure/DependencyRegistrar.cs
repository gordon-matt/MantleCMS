using Autofac;
using Mantle.Infrastructure;
using Mantle.Tenants.Services;
using Microsoft.Extensions.Configuration;

namespace Mantle.Tenants.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        builder.RegisterType<TenantService>().As<ITenantService>().InstancePerDependency();
    }

    public int Order => 0;

    #endregion IDependencyRegistrar Members
}