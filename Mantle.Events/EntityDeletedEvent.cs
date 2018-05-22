using Extenso.Data.Entity;

namespace Mantle.Events
{
    /// <summary>
    /// A container for passing entities that have been deleted. This is not used for entities that are deleted logicaly via a bit column.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityDeletedEvent<T> where T : IEntity
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="entity">Entity</param>
        public EntityDeletedEvent(T entity)
        {
            this.Entity = entity;
        }

        /// <summary>
        /// Entity
        /// </summary>
        public T Entity { get; }
    }
}