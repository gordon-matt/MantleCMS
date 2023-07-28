namespace Mantle.Tasks.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
    {
        builder.RegisterType<ScheduledTaskService>().As<IScheduledTaskService>().InstancePerDependency();
    }

    public int Order => 0;

    #endregion IDependencyRegistrar Members
}