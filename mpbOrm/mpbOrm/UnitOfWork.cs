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
    using mpbOrm.NpgsqlProvider;
    using System;
    using System.Data;

    /// <summary>
    /// All data access should go through this class.
    /// To override conventions inherit this class in your project and make the configuration changes
    /// necessary
    /// </summary>
    public abstract class UnitOfWork : IUnitOfWork
    {
        // Removed the DbProviderFactory because it has issues when using the Npgsql on Mono
        //private DbProviderFactory dbProviderFactory; 
        private string connectionString;

        private EntityMapContainer EntityMaps { get; set; }

        /// <summary>
        /// The DbProvider used in this unit of work
        /// </summary>
        protected IDbProvider DbProvider { get; set; }

        /// <summary>
        /// The current open transaction
        /// </summary>
        public DomainTransaction DomainTransaction { get; set; }

        /// <summary>
        /// The object cache used to ensure that entities are only loaded once
        /// </summary>
        public EntityCache EntityCache { get; set; }

        /// <summary>
        /// The LazyLoader sets up lazy loading for entities
        /// </summary>
        public LazyLoader LazyLoader { get; set; }

        /// <summary>
        /// The connection string used to connect to the database
        /// </summary>
        public string ConnectionString { get { return connectionString; } }

        /// <summary>
        /// Instanciates a new unit of work
        /// </summary>
        /// <param name="connectionString">Connection string used to connect to the database</param>
        /// <param name="dbProvider">The DbProvider used in this unit of work</param>
        /// <param name="entityCache">
        /// An object cache to be used with this unit of work. 
        /// If it is null the unit of work will instanciate its own cache
        /// </param>
        public UnitOfWork(string connectionString, string dbProviderName, EntityCache entityCache)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("connectionString cannot be empty", "connectionString");
            if (string.IsNullOrEmpty(dbProviderName))
                throw new ArgumentException("dbProviderName cannot be empty", "dbProvider");
            this.connectionString = connectionString;
            if (string.Equals(dbProviderName, "Npgsql", StringComparison.OrdinalIgnoreCase))
                this.DbProvider = new NpgsqlDbProvider(this);
            else
                throw new ArgumentException(string.Format("The DbProvider {0} is not recognized", dbProviderName), "dbProviderName");
            if (entityCache == null)
                this.EntityCache = new EntityCache();
            else
                this.EntityCache = entityCache;
            this.LazyLoader = new LazyLoader(this);
            this.EntityMaps = new EntityMapContainer();
        }

        /// <summary>
        /// Returns a repository
        /// </summary>
        /// <typeparam name="TEntity">The entity type that the repository works with</typeparam>
        /// <returns></returns>
        public IRepository<TEntity> Repo<TEntity>()
            where TEntity : IEntity
        {
            return this.DbProvider.Repo<TEntity>();
        }

        public EntityMap<TEntity> Map<TEntity>()
            where TEntity : IEntity
        {
            return this.EntityMaps.Map<TEntity>();
        }

        public EntityMap<TEntity> Map<TEntity>(string tableName)
            where TEntity : IEntity
        {
            return this.EntityMaps.Map<TEntity>(tableName);
        }

        /// <summary>
        /// Begins a transaction
        /// </summary>
        /// <returns>An open transaction</returns>
        public virtual IDomainTransaction BeginTransaction()
        {
            return this.DbProvider.BeginTransaction();
        }

        /// <summary>
        /// Returns a database connection
        /// </summary>
        /// <returns>A database connection</returns>
        public IDbConnection GetConnection()
        {
            if (this.DomainTransaction != null)
                return this.DomainTransaction.Connection;
            else
            {
                var connection = this.CreateConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                return connection;
            }
        }

        /// <summary>
        /// Closes the database connection
        /// </summary>
        /// <param name="connection">The connection to be closed</param>
        public void Close(IDbConnection connection)
        {
            if (this.DomainTransaction == null || !object.ReferenceEquals(connection, this.DomainTransaction.Connection))
                connection.Close();
        }

        /// <summary>
        /// Creates a new database connection
        /// </summary>
        /// <returns>A database connection</returns>
        protected IDbConnection CreateConnection()
        {
            return this.DbProvider.CreateConnection();
        }
    }
}
