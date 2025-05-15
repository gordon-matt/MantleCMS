using Dependo;
using Dependo.Lamar;
using Lamar;
using Microsoft.Extensions.Configuration;

namespace Mantle.Web.Infrastructure.Lamar;

public class DependencyRegistrar : BaseMantleWebDependencyRegistrar, ILamarDependencyRegistrar
{
    public void Register(ServiceRegistry services, ITypeFinder typeFinder, IConfiguration configuration) =>
        services.RegisterSettings(typeFinder);
}