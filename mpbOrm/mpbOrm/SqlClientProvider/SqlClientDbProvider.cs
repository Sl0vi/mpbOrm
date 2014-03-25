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

namespace mpbOrm.SqlClientProvider
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// DbProvider for Microsoft SQL Server 2012
    /// </summary>
    class SqlClientDbProvider : IDbProvider
    {
        /// <summary>
        /// The unit of work associated with the provider
        /// </summary>
        public UnitOfWork UnitOfWork { get; set; }

        public SqlClientDbProvider(UnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// Returns a new SqlConnection instance.
        /// </summary>
        public IDbConnection CreateConnection()
        {
            return new SqlConnection();
        }

        public IRepository<TEntity> Repo<TEntity>() where TEntity : IEntity
        {
            return new SqlClientRepository<TEntity>(this.UnitOfWork);
        }

        /// <summary>
        /// Opens a new connection to the database and starts a new transaction
        /// </summary>
        public IDbTransaction BeginTransaction()
        {
            if (this.UnitOfWork.DomainTransaction != null)
                throw new InvalidOperationException("A transaction is already open. Only one transaction can be open at a time");
            var connection = this.CreateConnection();
            connection.ConnectionString = this.UnitOfWork.ConnectionString;
            connection.Open();
            var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            return transaction;
        }
    }
}
