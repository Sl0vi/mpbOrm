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
    /// Contract for all implementations of repositories in the ORM
    /// 
    /// Repositories handle all data access actions for entities
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that the repository works with</typeparam>
    public interface IRepository<TEntity>
        where TEntity : IEntity
    {
        /// <summary>
        /// Returns a single entity or null if it doesn't exist
        /// </summary>
        /// <param name="id">The unique identifier of the entity</param>
        /// <returns></returns>
        TEntity FindById(Guid id);

        /// <summary>
        /// Returns a single entity based on the provided filter
        /// </summary>
        /// <param name="filter">The filter used to find the entity</param>
        /// <param name="param">An object with all parameters used in the filter</param>
        /// <returns></returns>
        TEntity Single(string filter = "", object param = null);

        /// <summary>
        /// Returns a list of entities
        /// </summary>
        /// <param name="filter">A filter used to limit the returned results</param>
        /// <param name="param">Parameters used in the filter</param>
        /// <param name="orderBy">The field that the result should be sorted by</param>
        /// <returns>A list of entities</returns>
        List<TEntity> Where(string filter = "", object param = null, string orderBy = "");

        /// <summary>
        /// Returns a paged result of entities
        /// </summary>
        /// <param name="filter">A filter used to limit the returned results</param>
        /// <param name="param">Parameters used in the filter</param>
        /// <param name="page">The page that should be returned</param>
        /// <param name="pageSize">The size of a page</param>
        /// <param name="orderBy">The field that the result should be sorted by</param>
        /// <returns>A paged result of entities</returns>
        PagedResult<TEntity> Paged(string filter = "", object param = null, int page = 1, int pageSize = 10, string orderBy = "");

        /// <summary>
        /// Adds an entity to the repository
        /// </summary>
        /// <param name="entity">The entity to be added</param>
        void Add(TEntity entity);

        /// <summary>
        /// Persists changes to an entity in the repository
        /// </summary>
        /// <param name="entity">The entity to be updated</param>
        void Update(TEntity entity);

        /// <summary>
        /// Removed an entity from the repository
        /// </summary>
        /// <param name="entity">The entity to be removed</param>
        void Remove(TEntity entity);
    }
}
