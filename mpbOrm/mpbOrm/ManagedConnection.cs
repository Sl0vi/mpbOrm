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
    /// This class provides shared functionality for managing database connections
    /// </summary>
    public abstract class ManagedConnection
    {
        protected string connectionString;
        //protected DbProviderFactory dbProviderFactory;
        protected UnitOfWork unitOfWork;

        /// <summary>
        /// Instantiates a new managed connection object
        /// </summary>
        /// <param name="connectionString">The connection string to the database</param>
        /// <param name="providerName">The provider name</param>
        protected ManagedConnection(string connectionString, string providerName)
        {
            this.connectionString = connectionString;
            //this.dbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        /// <summary>
        /// Intantiates a new managed connection object
        /// </summary>
        /// <param name="unitOfWork">The unit of work associated with this managed connection object</param>
        protected ManagedConnection(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Returns a database connection
        /// </summary>
        /// <returns>A database connection</returns>
        protected IDbConnection GetConnection()
        {
            if (unitOfWork != null)
                return unitOfWork.GetConnection();
            else
            {
                var connection = this.CreateConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                return connection;
            }
        }

        /// <summary>
        /// Creates a new database connection
        /// </summary>
        /// <returns>A new database connection</returns>
        protected abstract IDbConnection CreateConnection();

        /// <summary>
        /// Closes the connection to the database
        /// </summary>
        /// <param name="connection">The connection to be closed</param>
        protected void Close(IDbConnection connection)
        {
            if (unitOfWork == null)
                connection.Close();
            else
                unitOfWork.Close(connection);
        }
    }
}
