using Dependo;
using Dependo.DotNetDefault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.Infrastructure.DotNetDefault;

public class DependencyRegistrar : BaseMantleWebDependencyRegistrar, IDotNetDefaultDependencyRegistrar
{
    public void Register(IServiceCollection services, ITypeFinder typeFinder, IConfiguration configuration) =>
        services.RegisterSettings(typeFinder);
}