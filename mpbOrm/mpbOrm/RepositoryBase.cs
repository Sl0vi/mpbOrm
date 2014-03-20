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
    using Dapper;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Inherit this class to implement a repository
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the repository works with</typeparam>
    public abstract class RepositoryBase<TEntity>
       where TEntity : IEntity
    {
        protected UnitOfWork unitOfWork;

        /// <summary>
        /// Instanciates a new ReposityBase
        /// </summary>
        /// <param name="unitOfWork">The unit of work associated with this repository</param>
        protected RepositoryBase(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Returns a database connection
        /// </summary>
        /// <returns>A database connection</returns>
        protected IDbConnection GetConnection()
        {
            return unitOfWork.GetConnection();
        }

        /// <summary>
        /// Executes a query that returns results that are not entities in the domain
        /// </summary>
        /// <typeparam name="T">The type to be returned</typeparam>
        protected List<T> QueryNonEntity<T>(string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var connection = this.GetConnection();
            try
            {
                return (List<T>)SqlMapper.Query<T>(connection, sql, param, transaction, buffered, commandTimeout, commandType);
            }
            finally
            {
                unitOfWork.Close(connection);
            }
        }

        /// <summary>
        /// Executes a query that returns a single result of a type that is not an entity in the domain
        /// </summary>
        /// <typeparam name="T">The type to be returned</typeparam>
        protected T QueryNonEntitySingle<T>(string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var connection = this.GetConnection();
            try
            {
                return ((List<T>)SqlMapper.Query<T>(connection, sql, param, transaction, buffered, commandTimeout, commandType)).SingleOrDefault();
            }
            finally
            {
                unitOfWork.Close(connection);
            }
        }

        /// <summary>
        /// Executes a query that returns a collection of entities
        /// </summary>
        /// <typeparam name="T">The type of entities that are returned</typeparam>
        protected List<T> Query<T>(string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
            where T : IEntity
        {
            var connection = this.GetConnection();
            try
            {
                return this.LoadCollection(SqlMapper.Query<T>(connection, sql, param, transaction, buffered, commandTimeout, commandType));
            }
            finally
            {
                unitOfWork.Close(connection);
            }
        }

        /// <summary>
        /// Executes a query that returns a single entity
        /// </summary>
        /// <typeparam name="T">The type of entity that is returned</typeparam>
        protected T QuerySingle<T>(string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
            where T : IEntity
        {
            var connection = this.GetConnection();
            try
            {
                return this.Load(((List<T>)SqlMapper.Query<T>(connection, sql, param, transaction, buffered, commandTimeout, commandType)).SingleOrDefault());
            }
            finally
            {
                unitOfWork.Close(connection);
            }
        }

        /// <summary>
        /// Executes a command that returns no results
        /// </summary>
        protected void Execute(string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var connection = this.GetConnection();
            try
            {
                SqlMapper.Execute(connection, sql, param, transaction, commandTimeout, commandType);
            }
            finally
            {
                unitOfWork.Close(connection);
            }
        }

        /// <summary>
        /// Ensures that a collection of entities is properly loaded, by switching out results that are already in the 
        /// object cache and sets up lazy loading.
        /// </summary>
        /// <typeparam name="T">The type of entities that are loaded</typeparam>
        /// <param name="collection">The collection of entities</param>
        /// <returns>A properly loaded collection of entities</returns>
        protected virtual List<T> LoadCollection<T>(IEnumerable<T> collection)
            where T : IEntity
        {
            var loadedCollection = new List<T>();
            foreach (var entity in collection)
                loadedCollection.Add(this.Load(entity));
            return loadedCollection;
        }

        /// <summary>
        /// Ensures that a single entity is properly loaded by checking if it is already in the entity cache and set up
        /// lazy loading on the entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual T Load<T>(T entity)
            where T : IEntity
        {
            var cachedEntity = unitOfWork.EntityCache.Map<T>().Get(entity.Id);
            if (cachedEntity == null)
            {
                //if (this.unitOfWork != null)
                //    LazyLoader.Init<T>(entity, this.unitOfWork);
                unitOfWork.EntityCache.Map<T>().Add(entity);
                cachedEntity = entity;
            }
            return cachedEntity;
        }

        protected string Parse(string str)
        {
            return Regex.Replace(str, @"\{.+\}", new MatchEvaluator((match) =>
            {
                var propertyInfo = typeof(TEntity).GetProperties(BindingFlags.Public)
                    .SingleOrDefault(x => x.Name == match.Value.Substring(1, match.Value.Length - 2));
                if (propertyInfo != null)
                    return map.ColumnName(propertyInfo);
                else
                    return match.Value;
            }), RegexOptions.CultureInvariant | RegexOptions.Multiline);
        }
    }
}
