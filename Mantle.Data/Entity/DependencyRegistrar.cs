using Autofac;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Mantle.Data.Entity;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
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
                builder.RegisterType(config).As(typeof(IMantleEntityTypeConfiguration)).InstancePerLifetimeScope();
            }
        }
    }

    public int Order
    {
        get { return 0; }
    }

    #endregion IDependencyRegistrar Members
}