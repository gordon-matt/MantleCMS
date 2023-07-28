using Mantle.Plugins;
using Microsoft.Extensions.Logging;

namespace Mantle.Events;

/// <summary>
/// Evnt publisher
/// </summary>
public class EventPublisher : IEventPublisher
{
    private readonly ISubscriptionService _subscriptionService;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="subscriptionService"></param>
    public EventPublisher(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    /// <summary>
    /// Publish to cunsumer
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="x">Event consumer</param>
    /// <param name="eventMessage">Event message</param>
    protected virtual void PublishToConsumer<T>(IConsumer<T> x, T eventMessage)
    {
        try
        {
            x.HandleEvent(eventMessage);
        }
        catch (Exception ex)
        {
            //log error
            var loggerFactory = EngineContext.Current.Resolve<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(this.GetType());

            //we put in to nested try-catch to prevent possible cyclic (if some error occurs)
            try
            {
                logger.LogError(new EventId(), ex, ex.Message);
            }
            catch (Exception)
            {
                //do nothing
            }
        }
    }

    /// <summary>
    /// Publish event
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="eventMessage">Event message</param>
    public virtual void Publish<T>(T eventMessage)
    {
        //get all event subscribers, excluding from not installed plugins
        var subscribers = _subscriptionService.GetSubscriptions<T>()
            .Where(subscriber => PluginManager.FindPlugin(subscriber.GetType())?.Installed ?? true).ToList();

        //publish event to subscribers
        subscribers.ForEach(subscriber => PublishToConsumer(subscriber, eventMessage));
    }
}