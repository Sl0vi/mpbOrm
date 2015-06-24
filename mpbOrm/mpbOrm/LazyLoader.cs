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

    /// <summary>
    /// The lazy loader searches for properties of the type Lazy<TEntity> or Lazy<List<TEntity>> and
    /// sets up lazy loading based on conventions.
    /// The conventions can be overridden in the unit of work.
    /// </summary>
    public class LazyLoader
    {
        /// <summary>
        /// The unit of work associated with this lazy loader
        /// </summary>
        public IUnitOfWork UnitOfWork { get; private set; }

        /// <summary>
        /// Instanciates a new instance of the lazy loader
        /// </summary>
        /// <param name="unitOfWork">The unit of work associated with this lazy loader</param>
        public LazyLoader(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// Sets up lazy loading for an entity
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity</typeparam>
        /// <param name="entity">The entity to set up lazy loading for</param>
        public void Init<TEntity>(TEntity entity)
        {
            var properties = typeof(TEntity).GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType.IsGenericType
                    && property.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Lazy<>))
                    && property.GetValue(entity, null) == null)
                {
                    var genericArgs = property.PropertyType.GetGenericArguments();
                    if (genericArgs[0].IsGenericType
                        && genericArgs[0].GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
                    {
                        var methodInfo = typeof(LazyLoader).GetMethod("LoadCollection", BindingFlags.NonPublic | BindingFlags.Instance);
                        var genericMethod = methodInfo.MakeGenericMethod(new Type[] { property.PropertyType.GetGenericArguments()[0].GetGenericArguments()[0] });
                        property.SetValue(entity, genericMethod.Invoke(this, new object[] { entity, property }), null);
                    }
                    else
                    {
                        var methodInfo = typeof(LazyLoader).GetMethod("LoadSingle", BindingFlags.NonPublic | BindingFlags.Instance);
                        var genericMethod = methodInfo.MakeGenericMethod(new Type[] { property.PropertyType.GetGenericArguments()[0] });
                        property.SetValue(entity, genericMethod.Invoke(this, new object[] { entity, property }), null);
                    }
                }
            }
        }

        /// <summary>
        /// Sets up lazy loading for a property that returns a single entity
        /// </summary>
        /// <typeparam name="T">The type of entity to lazy load</typeparam>
        /// <param name="target">The entity that the property belongs to</param>
        /// <param name="propertyInfo">The property that is being lazy loaded</param>
        /// <returns>A instance of Lazy<></returns>
        private Lazy<TEntity> LoadSingle<TEntity>(object target, PropertyInfo propertyInfo)
        {
//            return new Lazy<TEntity>(() =>
//                {
//                    var idOfTarget = (Guid)target.GetType().GetProperty(propertyInfo.Name + "Id").GetValue(target, null);
//                    return this.UnitOfWork.Repo<TEntity>().FindById(idOfTarget);
//                });
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets up lazy loading for a property that returns a list
        /// </summary>
        /// <typeparam name="T">The type of entity to lazy load</typeparam>
        /// <param name="target">The entity that the property belongs to</param>
        /// <param name="propertyInfo">The property that is being lazy loaded</param>
        /// <returns>An instance of Lazy<List<>></returns>
        private Lazy<List<TEntity>> LoadCollection<TEntity>(object target, PropertyInfo propertyInfo)
        {
//            return new Lazy<List<TEntity>>(() =>
//                {
//                    return this.UnitOfWork.Repo<TEntity>().Where(
//                        string.Format("{{{0}Id}} = @Id", target.GetType().Name),
//                        target);
//                });
            throw new NotImplementedException();
        }
    }
}
