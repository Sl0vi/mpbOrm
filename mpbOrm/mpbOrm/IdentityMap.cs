// The MIT License (MIT)
//
// Copyright (c) 2014 Bernhard Johannessen
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
