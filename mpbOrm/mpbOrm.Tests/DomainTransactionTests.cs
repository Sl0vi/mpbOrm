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

namespace mpbOrm.Tests
{
    using Moq;
    using mpbOrm.Tests.TestClasses;
    using NUnit.Framework;
    using System;
    using System.Data;

    [TestFixture]
    public class DomainTransactionTests
    {
        [Test]
        public void CanCommit()
        {
            var dbTransaction = new Mock<IDbTransaction>();
            var dbConnection = new Mock<IDbConnection>();
            var dbProvider = new Mock<IDbProvider>();
            var unitOfWork = new Mock<UnitOfWork>("fakeConnectionString", dbProvider.Object, null);
            dbTransaction.Setup(x => x.Connection).Returns(dbConnection.Object);
            var domainTransaction = new DomainTransaction(dbConnection.Object, dbTransaction.Object, unitOfWork.Object);
            domainTransaction.Commit();
            dbTransaction.Verify(x => x.Commit(), Times.Once());
        }

        [Test]
        public void RollsBackOnFailure()
        {
            var dbTransaction = new Mock<IDbTransaction>();
            var dbConnection = new Mock<IDbConnection>();
            var dbProvider = new Mock<IDbProvider>();
            var unitOfWork = new Mock<UnitOfWork>("fakeConnectionString", dbProvider.Object, null);
            dbTransaction.Setup(x => x.Connection).Returns(dbConnection.Object);
            dbTransaction.Setup(x => x.Commit()).Throws(new Exception());
            var domainTransaction = new DomainTransaction(dbConnection.Object, dbTransaction.Object, unitOfWork.Object);
            Assert.Throws<Exception>(() =>
                {
                    domainTransaction.Commit();
                });
            dbTransaction.Verify(x => x.Rollback(), Times.Once());
        }

        [Test]
        public void ProperlyDisposes()
        {
            var dbTransaction = new Mock<IDbTransaction>();
            var dbConnection = new Mock<IDbConnection>();
            var dbProvider = new Mock<IDbProvider>();
            var unitOfWork = new FakeUnitOfWork("fakeConnectionString", dbProvider.Object, null);
            var domainTransaction = new DomainTransaction(dbConnection.Object, dbTransaction.Object, unitOfWork);
            domainTransaction.Dispose();
            dbConnection.Verify(x => x.Close(), Times.Once());
        }

        [Test]
        public void ProperlyDisposesWhenCreatedFromBeginTransactionOnUnitOfWork()
        {
            var dbTransaction = new Mock<IDbTransaction>();
            var dbConnection = new Mock<IDbConnection>();
            var dbProvider = new Mock<IDbProvider>();
            var unitOfWork = new FakeUnitOfWork("fakeConnectionString", dbProvider.Object, null);
            dbTransaction.Setup(x => x.Connection).Returns(dbConnection.Object);
            dbProvider.Setup(x => x.BeginTransaction()).Returns(() => 
                {
                    return dbTransaction.Object;
                });
            var domainTransaction = unitOfWork.BeginTransaction();
            domainTransaction.Dispose();
            dbConnection.Verify(x => x.Close(), Times.Once());
            Assert.That(unitOfWork.DomainTransaction, Is.Null);
        }
    }
}
