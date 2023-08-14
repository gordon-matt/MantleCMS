using Autofac;
using Microsoft.Extensions.Configuration;

namespace Mantle.Events.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    public void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
        builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().SingleInstance();
    }

    public int Order => 0;
}