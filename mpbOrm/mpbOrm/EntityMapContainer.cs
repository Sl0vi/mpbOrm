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
    using System.Reflection;

    public class EntityMapContainer
    {
        public Dictionary<Type, object> EntityMaps { get; set; }

        public EntityMapContainer()
        {
            this.EntityMaps = new Dictionary<Type, object>();
        }

        public EntityMap<TEntity> Map<TEntity>()
            where TEntity : IEntity
        {
            object mapObject;
            if (!this.EntityMaps.TryGetValue(typeof(TEntity), out mapObject))
                this.EntityMaps[typeof(TEntity)] = mapObject = new EntityMap<TEntity>();
            return (EntityMap<TEntity>)mapObject;
        }

        public EntityMap<TEntity> Map<TEntity>(string tableName)
            where TEntity : IEntity
        {
            object mapObject;
            if (!this.EntityMaps.TryGetValue(typeof(TEntity), out mapObject))
                this.EntityMaps[typeof(TEntity)] = mapObject = new EntityMap<TEntity>();
            var entityMap = mapObject as EntityMap<TEntity>;
            entityMap.TableName = tableName;
            return entityMap;
        }
    }
}
