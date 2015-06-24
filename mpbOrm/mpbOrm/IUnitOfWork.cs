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
    /// <summary>
    /// The unit of work provides access to repositories and can wrap all data
    /// access calls in a transaction.
    /// </summary>
    public interface IUnitOfWork
    {
        IDbProvider DbProvider { get; }

        /// <summary>
        /// Returns a repository with data access methods for the specified type.
        /// </summary>
        /// <typeparam name="TEntity">The type the the returned repository works with</typeparam>
        IRepository<TEntity, TKey> Repo<TEntity, TKey>();

        /// <summary>
        /// Starts a transaction, all data access calls through the unit of work will be wrapped in this transaction.
        /// </summary>
        /// <returns>An open transaction</returns>
        IDomainTransaction BeginTransaction();
    }
}
