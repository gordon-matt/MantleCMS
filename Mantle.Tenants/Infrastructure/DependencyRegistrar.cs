using Mantle.Tenants.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Tenants.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(IContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration) =>
        builder.Register<ITenantService, TenantService>(ServiceLifetime.Transient);

    public int Order => 0;

    #endregion IDependencyRegistrar Members
}