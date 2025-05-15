namespace Mantle.Events;

/// <summary>
/// Event subscription service
/// </summary>
public class SubscriptionService : ISubscriptionService
{
    /// <summary>
    /// Get subscriptions
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <returns>Event consumers</returns>
    public IList<IConsumer<T>> GetSubscriptions<T>() => DependoResolver.Instance.ResolveAll<IConsumer<T>>().ToList();
}