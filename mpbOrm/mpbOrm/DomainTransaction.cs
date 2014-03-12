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
    using System.Data;

    /// <summary>
    /// Implementation of domain transactions using ADO.Net
    /// </summary>
    public class DomainTransaction : IDomainTransaction
    {
        /// <summary>
        /// The unit of work that started this transaction
        /// </summary>
        public UnitOfWork UnitOfWork { get; private set; }

        /// <summary>
        /// The database connection
        /// </summary>
        public IDbConnection Connection { get; private set; }

        /// <summary>
        /// The database transaction
        /// </summary>
        public IDbTransaction Transaction { get; private set; }

        /// <summary>
        /// Instanciates a new DomainTransaction
        /// </summary>
        /// <param name="connection">The database connection</param>
        /// <param name="transaction">The database transaction</param>
        /// <param name="unitOfWork">The unit of work that created the transaction</param>
        public DomainTransaction(IDbConnection connection, IDbTransaction transaction, UnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
            this.Connection = connection;
            this.Transaction = transaction;
        }

        /// <summary>
        /// Commits the transaction to the database or rolls back if the commit fails.
        /// </summary>
        public void Commit()
        {
            try
            {
                this.Transaction.Commit();
            }
            catch
            {
                this.Transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Disposes of the DomainTransaction
        /// </summary>
        public void Dispose()
        {
            if (this.Connection != null)
            {
                this.Connection.Close();
                this.Connection = null;
            }
            if (this.UnitOfWork.DomainTransaction != null && object.ReferenceEquals(this, this.UnitOfWork.DomainTransaction))
                this.UnitOfWork.DomainTransaction = null;
        }
    }
}
