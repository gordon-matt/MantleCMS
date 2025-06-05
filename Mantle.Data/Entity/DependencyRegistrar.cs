using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Data.Entity;

public class DependencyRegistrar : IDependencyRegistrar
{
    public void Register(IContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        var entityTypeConfigurations = typeFinder
            .FindClassesOfType(typeof(IMantleEntityTypeConfiguration))
            .ToHashSet();

        foreach (var config in entityTypeConfigurations)
        {
            if (config.GetTypeInfo().IsGenericType)
            {
                continue;
            }

            bool isEnabled = (Activator.CreateInstance(config) as IMantleEntityTypeConfiguration).IsEnabled;

            if (isEnabled)
            {
                builder.Register(typeof(IMantleEntityTypeConfiguration), config, ServiceLifetime.Scoped);
            }
        }
    }

    public int Order => 0;
}