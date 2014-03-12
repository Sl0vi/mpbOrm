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
