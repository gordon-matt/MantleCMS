using Dependo;
using Dependo.DryIoc;
using DryIoc;
using Microsoft.Extensions.Configuration;

namespace Mantle.Web.Infrastructure.DryIoc;

public class DependencyRegistrar : BaseMantleWebDependencyRegistrar, IDryIocDependencyRegistrar
{
    public void Register(IContainer container, ITypeFinder typeFinder, IConfiguration configuration) =>
        container.RegisterSettings(typeFinder);
}