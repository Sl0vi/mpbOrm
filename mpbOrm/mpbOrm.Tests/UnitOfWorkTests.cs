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
    using mpbOrm.NpgsqlProvider;
    using mpbOrm.SqlClientProvider;
    using mpbOrm.Tests.TestClasses;
    using NUnit.Framework;
    using System;
    using System.Data;

    [TestFixture]
    public class UnitOfWorkTests
    {
        [Test]
        public void CanInstantiateWithNpgsqlDbProviderName()
        {
            UnitOfWork unitOfWork = null;
            Assert.DoesNotThrow(() =>
                {
                    unitOfWork = new UnitOfWork("fakeConnectionString", "Npgsql");
                });
            Assert.That(unitOfWork.DbProvider, Is.TypeOf<NpgsqlDbProvider>());
        }

        [Test]
        public void CanInstantiateWithSqlClientDbProviderName()
        {
            UnitOfWork unitOfWork = null;
            Assert.DoesNotThrow(() =>
                {
                    unitOfWork = new UnitOfWork("fakeConnectionString", "System.Data.SqlClient");
                });
            Assert.That(unitOfWork.DbProvider, Is.TypeOf<SqlClientDbProvider>());
        }

        [Test]
        public void InstantiationThrowsOnUnkownDbProviderName()
        {
            UnitOfWork unitOfWork = null;
            Assert.Throws<ArgumentException>(() =>
                {
                    unitOfWork = new UnitOfWork("fakeConnectionString", "NoSuchProvider");
                });
        }

        [Test]
        public void CanPassInDbProviderInContructor()
        {
            var providerMock = new Mock<IDbProvider>();
            UnitOfWork unitOfWork = null;
            Assert.DoesNotThrow(() =>
                {
                    unitOfWork = new UnitOfWork("fakeConnectionString", providerMock.Object);
                });
            Assert.That(unitOfWork.DbProvider, Is.SameAs(providerMock.Object));
        }

        [Test]
        public void CanPassInEntityCacheIntoConstructorOutsideUnitOfWork()
        {
            var providerMock = new Mock<IDbProvider>();
            var entityCache = new EntityCache();
            UnitOfWork unitOfWork = null;
            Assert.DoesNotThrow(() =>
                {
                    unitOfWork = new UnitOfWork("fakeConnectionString", providerMock.Object, entityCache);
                });
            Assert.That(unitOfWork.EntityCache, Is.SameAs(entityCache));
        }

        [Test]
        public void RepoReturnsRepository()
        {
            var repositoryMock = new Mock<IRepository<TestEntity>>();
            var providerMock = new Mock<IDbProvider>();
            providerMock.Setup(x => x.Repo<TestEntity>()).Returns(repositoryMock.Object);
            var unitOfWork = new UnitOfWork("fakeConnectionString", providerMock.Object);
            var repository = unitOfWork.Repo<TestEntity>();
            providerMock.Verify(x => x.Repo<TestEntity>(), Times.Once());
            Assert.That(repository, Is.Not.Null);
            Assert.That(repository, Is.SameAs(repositoryMock.Object));
        }

        [Test]
        public void MapReturnsEntityMap()
        {
            var providerMock = new Mock<IDbProvider>();
            var unitOfWork = new UnitOfWork("fakeConnectionString", providerMock.Object);
            var entityMap = unitOfWork.Map<TestEntity>();
            Assert.That(entityMap, Is.Not.Null);
        }

        [Test]
        public void MapCanRetunEntityMapAndSetTableNameAtTheSameTime()
        {
            var providerMock = new Mock<IDbProvider>();
            var unitOfWork = new UnitOfWork("fakeConnectionString", providerMock.Object);
            var entityMap = unitOfWork.Map<TestEntity>("TestTableName");
            Assert.That(entityMap, Is.Not.Null);
            Assert.That(entityMap.TableName, Is.EqualTo("TestTableName"));
        }

        [Test]
        public void BeginTransactionReturnsNewDomainTransaction()
        {
            var connectionMock = new Mock<IDbConnection>();
            var transactionMock = new Mock<IDbTransaction>();
            transactionMock.Setup(x => x.Connection).Returns(connectionMock.Object);
            var providerMock = new Mock<IDbProvider>();
            providerMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);
            var unitOfWork = new UnitOfWork("fakeConnectionString", providerMock.Object);
            IDomainTransaction domainTransaction = null;
            Assert.DoesNotThrow(() =>
                {
                    domainTransaction = unitOfWork.BeginTransaction();
                });
            Assert.That(domainTransaction, Is.Not.Null);
            Assert.That(domainTransaction, Is.TypeOf<DomainTransaction>());
            var domainTransactionClass = domainTransaction as DomainTransaction;
            Assert.That(domainTransactionClass.Connection, Is.SameAs(connectionMock.Object));
            Assert.That(domainTransactionClass.Transaction, Is.SameAs(transactionMock.Object));
        }

        [Test]
        public void CanOnlyHaveOneTransactionOpenAtTheSameTime()
        {
            var connectionMock = new Mock<IDbConnection>();
            var transactionMock = new Mock<IDbTransaction>();
            transactionMock.Setup(x => x.Connection).Returns(connectionMock.Object);
            var providerMock = new Mock<IDbProvider>();
            providerMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);
            var unitOfWork = new UnitOfWork("fakeConnectionString", providerMock.Object);
            var transaction = unitOfWork.BeginTransaction();
            Assert.That(transaction, Is.Not.Null);
            Assert.That(unitOfWork.DomainTransaction, Is.Not.Null);
            Assert.Throws<InvalidOperationException>(() =>
                {
                    unitOfWork.BeginTransaction();
                });
        }

        [Test]
        public void GetConnectionReturnsConnectionFromDbProviderIfNoTransactionIsOpen()
        {
            var connectionMock = new Mock<IDbConnection>();
            var providerMock = new Mock<IDbProvider>();
            providerMock.Setup(x => x.CreateConnection()).Returns(connectionMock.Object);
            var unitOfWork = new UnitOfWork("fakeConnectionString", providerMock.Object);
            var connection = unitOfWork.GetConnection();
            providerMock.Verify(x => x.CreateConnection(), Times.Once());
            Assert.That(connection, Is.Not.Null);
            Assert.That(connection, Is.SameAs(connectionMock.Object));
        }

        [Test]
        public void GetConnectionReturnsConnectionFromTransactionIfTransactionOpen()
        {
            var connectionMock = new Mock<IDbConnection>();
            var transactionMock = new Mock<IDbTransaction>();
            var providerMock = new Mock<IDbProvider>();
            connectionMock.Setup(x => x.State).Returns(ConnectionState.Open);
            connectionMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);
            transactionMock.Setup(x => x.Connection).Returns(connectionMock.Object);
            providerMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);
            var unitOfWork = new UnitOfWork("fakeConnectionString", providerMock.Object);
            var transaction = unitOfWork.BeginTransaction();
            providerMock.Verify(x => x.BeginTransaction(), Times.Once());
            Assert.That(transaction, Is.Not.Null);
        }

        [Test]
        public void CloseClosesConnection()
        {
            var connectionMock = new Mock<IDbConnection>();
            var providerMock = new Mock<IDbProvider>();
            var unitOfWork = new UnitOfWork("fakeConnectionString", providerMock.Object);
            unitOfWork.Close(connectionMock.Object);
            connectionMock.Verify(x => x.Close(), Times.Once());
        }

        [Test]
        public void CloseDoesNotCloseConnectionForOpenTransaction()
        {
            var connectionMock = new Mock<IDbConnection>();
            var transactionMock = new Mock<IDbTransaction>();
            var providerMock = new Mock<IDbProvider>();
            connectionMock.Setup(x => x.State).Returns(ConnectionState.Open);
            connectionMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);
            transactionMock.Setup(x => x.Connection).Returns(connectionMock.Object);
            providerMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);
            var unitOfWork = new UnitOfWork("fakeConnectionString", providerMock.Object);
            var transaction = unitOfWork.BeginTransaction();
            unitOfWork.Close(connectionMock.Object);
            connectionMock.Verify(x => x.Close(), Times.Never());
        }
    }
}
