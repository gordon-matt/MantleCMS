using Autofac;
using System.Reflection;

namespace Mantle.Data.Entity;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
    {
        var entityTypeConfigurations = typeFinder
            .FindClassesOfType(typeof(IMantleEntityTypeConfiguration))
            .ToHashSet();

        foreach (var configuration in entityTypeConfigurations)
        {
            if (configuration.GetTypeInfo().IsGenericType)
            {
                continue;
            }

            var isEnabled = (Activator.CreateInstance(configuration) as IMantleEntityTypeConfiguration).IsEnabled;

            if (isEnabled)
            {
                builder.RegisterType(configuration).As(typeof(IMantleEntityTypeConfiguration)).InstancePerLifetimeScope();
            }
        }
    }

    public int Order
    {
        get { return 0; }
    }

    #endregion IDependencyRegistrar Members
}