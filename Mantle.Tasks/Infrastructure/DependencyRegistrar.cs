using Microsoft.Extensions.Configuration;

namespace Mantle.Tasks.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration) =>
        builder.RegisterType<ScheduledTaskService>().As<IScheduledTaskService>().InstancePerDependency();

    public int Order => 0;

    #endregion IDependencyRegistrar Members
}