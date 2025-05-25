using Dependo;
using Dependo.LightInject;
using LightInject;
using Microsoft.Extensions.Configuration;

namespace Mantle.Web.Infrastructure.LightInject;

public class DependencyRegistrar : BaseMantleWebDependencyRegistrar, ILightInjectDependencyRegistrar
{
    public void Register(IServiceContainer container, ITypeFinder typeFinder, IConfiguration configuration) =>
        container.RegisterSettings(typeFinder);
}