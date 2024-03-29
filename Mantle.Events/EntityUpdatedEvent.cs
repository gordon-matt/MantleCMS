﻿namespace Mantle.Events;

/// <summary>
/// A container for entities that are updated.
/// </summary>
/// <typeparam name="T"></typeparam>
public class EntityUpdatedEvent<T> where T : IEntity
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="entity">Entity</param>
    public EntityUpdatedEvent(T entity)
    {
        Entity = entity;
    }

    /// <summary>
    /// Entity
    /// </summary>
    public T Entity { get; }
}