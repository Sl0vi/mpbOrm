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
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Inherit this class to implement a repository
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the repository works with</typeparam>
    public abstract class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey>
    {
        protected UnitOfWork UnitOfWork { get; private set; }
        protected EntityMap<TEntity> Map { get; private set; }
        protected IParser<TEntity> Parser { get; set; }

        /// <summary>
        /// Instanciates a new ReposityBase
        /// </summary>
        /// <param name="unitOfWork">The unit of work associated with this repository</param>
        protected RepositoryBase(UnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
            this.Map = this.UnitOfWork.Map<TEntity>();
            this.Parser = new Parser<TEntity>(this.Map);
        }

        public virtual TEntity FindById(TKey id)
        {
            var entity = this.UnitOfWork.EntityCache.Map<TEntity, TKey>().Get(id);
            if (entity == null)
            {
                entity = this.QuerySingle(
                    string.Format(
                        "SELECT {0} FROM {1} WHERE {2} = @Id",
                        string.Join(", ", from f in this.Map.ColumnNames() select this.Map.TableName + "." + f),
                        this.Map.TableName,
                        this.Map.KeyColumnName),
                    new { Id = id });
            }
            return entity;
        }

        public virtual TEntity Single(string filter = "", object param = null)
        {
            return ((List<TEntity>)this.Where(filter, param)).SingleOrDefault();
        }

        public virtual List<TEntity> Where(string filter = "", object param = null, string orderBy = "")
        {
            var parsedFilter = this.Parser.Parse(filter);
            var parsedOrderBy = this.Parser.Parse(orderBy);
            return this.Query(
                this.SelectBuilder(parsedFilter, parsedOrderBy).ToString(),
                param);
        }

        public virtual PagedResult<TEntity> Paged(string filter = "", object param = null, int page = 1, int pageSize = 10, string orderBy = "")
        {
            string parsedFilter = this.Parser.Parse(filter);
            string parsedOrderBy = this.Parser.Parse(orderBy);
            var builder = this.SelectBuilder(parsedFilter, parsedOrderBy);
            builder.Append(
                string.Format(
                    " OFFSET {0}",
                    (page - 1) * pageSize));
            builder.Append(
                string.Format(
                    " LIMIT {0}",
                    pageSize));
            var result = this.Query(builder.ToString(), param);
            var countBuilder = new StringBuilder();
            countBuilder.Append(
                string.Format(
                    "SELECT CAST(COUNT(*) AS INT) FROM {0}",
                    this.Map.TableName));
            if (!string.IsNullOrEmpty(parsedFilter))
                countBuilder.Append(
                    string.Format(
                        " WHERE {0}",
                        parsedFilter));
            var total = this.QueryNonEntitySingle<int>(countBuilder.ToString(), param);
            var pagedResult = new PagedResult<TEntity>
            {
                Page = page,
                PageSize = pageSize,
            };
            pagedResult.AddRange(result);
            return pagedResult;
        }

        public virtual void Add(TEntity entity)
        {
//            if (entity.Id == Guid.Empty)
//                entity.Id = Guid.NewGuid();
            this.Execute(
                string.Format(
                    "INSERT INTO {0}({1}) VALUES({2})",
                    this.Map.TableName,
                    string.Join(", ", this.Map.ColumnNames()),
                    string.Join(", ", from f in this.Map.Columns select "@" + f.Key.Name)),
                entity);
            this.Load(entity);
        }

        public virtual void Update(TEntity entity)
        {
//            this.Execute(
//                string.Format(
//                    "UPDATE {0} SET {1} WHERE {2} = @Id",
//                    this.Map.TableName,
//                    string.Join(", ", from f in this.Map.Columns where f.Key.Name != "Id" select string.Format("{0} = @{1}", f.Value, f.Key.Name)),
//                    this.Map.ColumnName(x => x.Id)),
//                entity);
        }

        public virtual void Remove(TEntity entity)
        {
//            this.Execute(
//                string.Format(
//                    "DELETE FROM {0} WHERE {1} = @Id",
//                    this.Map.TableName,
//                    this.Map.ColumnName(x => x.Id)),
//                entity);
//            UnitOfWork.EntityCache.Map<TEntity>().Remove(entity.Id);
        }

        protected virtual StringBuilder SelectBuilder(string parsedFilter = "", string parsedOrderBy = "")
        {
            var builder = new StringBuilder();
            builder.Append(
                string.Format(
                    "SELECT {0} FROM {1}",
                    string.Join(", ", from f in this.Map.ColumnNames() select this.Map.TableName + "." + f),
                    this.Map.TableName));
            if (!string.IsNullOrEmpty(parsedFilter))
                builder.Append(
                    string.Format(
                        " WHERE {0}",
                        parsedFilter));
            if (!string.IsNullOrEmpty(parsedOrderBy))
                builder.Append(
                    string.Format(
                        " ORDER BY {0}",
                        parsedOrderBy));
            return builder;
        }

        /// <summary>
        /// Returns a database connection
        /// </summary>
        /// <returns>A database connection</returns>
        protected IDbConnection GetConnection()
        {
            return UnitOfWork.GetConnection();
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
                UnitOfWork.Close(connection);
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
                UnitOfWork.Close(connection);
            }
        }

        /// <summary>
        /// Executes a query that returns a collection of entities
        /// </summary>
        /// <typeparam name="T">The type of entities that are returned</typeparam>
        protected List<TEntity> Query(string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var connection = this.GetConnection();
            try
            {
                return this.LoadCollection(SqlMapper.Query<TEntity>(connection, sql, param, transaction, buffered, commandTimeout, commandType));
            }
            finally
            {
                UnitOfWork.Close(connection);
            }
        }

        /// <summary>
        /// Executes a query that returns a single entity
        /// </summary>
        /// <typeparam name="T">The type of entity that is returned</typeparam>
        protected TEntity QuerySingle(string sql, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var connection = this.GetConnection();
            try
            {
                return this.Load(((List<TEntity>)SqlMapper.Query<TEntity>(connection, sql, param, transaction, buffered, commandTimeout, commandType)).SingleOrDefault());
            }
            finally
            {
                UnitOfWork.Close(connection);
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
                UnitOfWork.Close(connection);
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
        {
//            var loadedCollection = new List<T>();
//            foreach (var entity in collection)
//                loadedCollection.Add(this.Load(entity));
//            return loadedCollection;
            return null;
        }

        /// <summary>
        /// Ensures that a single entity is properly loaded by checking if it is already in the entity cache and set up
        /// lazy loading on the entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual TEntity Load(TEntity entity)
        {
//            var cachedEntity = UnitOfWork.EntityCache.Map<TEntity, TKey>().Get(entity.Id);
//            if (cachedEntity == null)
//            {
//                if (this.UnitOfWork != null)
//                    this.UnitOfWork.LazyLoader.Init<T>(entity);
//                UnitOfWork.EntityCache.Map<TEntity, TKey>().Add(entity);
//                cachedEntity = entity;
//            }
//            return cachedEntity;
            return default(TEntity);
        }
    }
}
