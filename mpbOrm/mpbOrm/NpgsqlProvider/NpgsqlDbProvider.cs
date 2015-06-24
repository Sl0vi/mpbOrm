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

namespace mpbOrm.NpgsqlProvider
{
    using Dapper;
    using Npgsql;
    using System.Data;

    /// <summary>
    /// DbProvider for PostgreSQL databases
    /// </summary>
    public class NpgsqlDbProvider : IDbProvider
    {
        /// <summary>
        /// The unit of work associated with the provider
        /// </summary>
        public UnitOfWork UnitOfWork { get; set; }

        public NpgsqlDbProvider(UnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// Returns a new instance of an NpgsqlConnection
        /// </summary>
        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection();
        }

        /// <summary>
        /// Returns a new repository for the specified entity type
        /// </summary>
        /// <typeparam name="TEntity">The type of entity the repository works with</typeparam>
        public IRepository<TEntity, TKey> Repo<TEntity, TKey>()
        {
            return new NpgsqlRepository<TEntity, TKey>(this.UnitOfWork);
        }

        /// <summary>
        /// Opens a connection to the database and starts a new transaction.
        /// All constrains are set to deferred for postgres transactions
        /// </summary>
        public IDbTransaction BeginTransaction()
        {
            var connection = this.CreateConnection();
            connection.ConnectionString = this.UnitOfWork.ConnectionString;
            connection.Open();
            var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            transaction.Connection.Execute("SET CONSTRAINTS ALL DEFERRED");
            return transaction;
        }
    }
}
