using System;
using System.Reflection;
using Autofac;
using Mantle.Collections;
using Mantle.Infrastructure;

namespace Mantle.Data.Entity.EntityFramework
{
    public class DependencyRegistrar : IDependencyRegistrar<ContainerBuilder>
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
}