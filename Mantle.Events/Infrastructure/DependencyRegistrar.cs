using Autofac;

namespace Mantle.Events.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
    {
        builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
        builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().SingleInstance();
    }

    public int Order => 0;
}