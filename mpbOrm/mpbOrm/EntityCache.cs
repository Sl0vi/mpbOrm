namespace mpbOrm
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The entity cache provides access to identity maps for all entity types.
    /// 
    /// It is used to ensure that any entity is only loaded into memory once inside a unit of work
    /// </summary>
    public class EntityCache
    {
        private readonly Dictionary<Type, object> maps = new Dictionary<Type, object>();

        /// <summary>
        /// Returns an identity map based on the specified entity type
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <returns>An identity map for the specified entity type</returns>
        public IdentityMap<TEntity> Map<TEntity>()
            where TEntity : IEntity
        {
            object mapObject;
            if (!maps.TryGetValue(typeof(TEntity), out mapObject))
                maps[typeof(TEntity)] = mapObject = new IdentityMap<TEntity>();
            return (IdentityMap<TEntity>)mapObject;
        }
    }
}
