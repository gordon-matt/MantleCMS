using Autofac;
using Dependo;
using Dependo.Autofac;
using Microsoft.Extensions.Configuration;

namespace Mantle.Web.Infrastructure.Autofac;

public class DependencyRegistrar : BaseMantleWebDependencyRegistrar, IAutofacDependencyRegistrar
{
    public void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        // Configuration
        builder.RegisterModule<ConfigurationModule>();
    }
}