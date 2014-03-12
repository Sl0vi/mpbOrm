namespace mpbOrm
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The identity map is a cache that ensures that any specific entity will only be loaded once
    /// </summary>
    /// <typeparam name="TEntity">The type of entities contained in the identity map</typeparam>
    public class IdentityMap<TEntity>
        where TEntity : IEntity
    {
        private readonly Dictionary<Guid, TEntity> entities = new Dictionary<Guid, TEntity>();
        private readonly object lockObject = new object();

        /// <summary>
        /// Tries to get an entity based on the specified unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the entity</param>
        /// <returns>An entity or default if no result was found</returns>
        public TEntity Get(Guid id)
        {
            lock (lockObject)
            {
                TEntity entity;
                if (!entities.TryGetValue(id, out entity))
                    return default(TEntity);
                return entity;
            }
        }

        /// <summary>
        /// Adds an entity to the identity map
        /// </summary>
        /// <param name="entity">The entity to be added</param>
        public void Add(TEntity entity)
        {
            lock (lockObject)
            {
                entities[entity.Id] = entity;
            }
        }

        /// <summary>
        /// Removes an entity from the identity map
        /// </summary>
        /// <param name="id">The unique identifier of the entity to be removed</param>
        public void Remove(Guid id)
        {
            lock (lockObject)
            {
                TEntity entity;
                if (entities.TryGetValue(id, out entity))
                    entities.Remove(id);
            }
        }
    }
}
