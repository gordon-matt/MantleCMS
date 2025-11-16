namespace Mantle.Events;

/// <summary>
/// Event publisher extensions
/// </summary>
public static class EventPublisherExtensions
{
    extension(IEventPublisher eventPublisher)
    {
        /// <summary>
        /// Entity inserted
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        public void EntityInserted<T>(T entity) where T : IEntity =>
            eventPublisher.Publish(new EntityInsertedEvent<T>(entity));

        /// <summary>
        /// Entity updated
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        public void EntityUpdated<T>(T entity) where T : IEntity =>
            eventPublisher.Publish(new EntityUpdatedEvent<T>(entity));

        /// <summary>
        /// Entity deleted
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        public void EntityDeleted<T>(T entity) where T : IEntity =>
            eventPublisher.Publish(new EntityDeletedEvent<T>(entity));
    }
}