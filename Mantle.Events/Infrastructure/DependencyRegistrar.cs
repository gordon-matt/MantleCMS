using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Events.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    public void Register(IContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        builder.Register<IEventPublisher, EventPublisher>(ServiceLifetime.Singleton);
        builder.Register<ISubscriptionService, SubscriptionService>(ServiceLifetime.Singleton);
    }

    public int Order => 0;
}