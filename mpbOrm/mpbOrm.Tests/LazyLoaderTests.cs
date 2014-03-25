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
    using System.Collections.Generic;

    [TestFixture]
    public class LazyLoaderTests
    {
        [Test]
        public void SetsUpLazyLoadingForLazyTypes()
        {
            // Setup
            var testCollection = new List<LazyLoadedEntity>
            {
                new LazyLoadedEntity { Id = Guid.NewGuid() },
                new LazyLoadedEntity { Id = Guid.NewGuid() },
                new LazyLoadedEntity { Id = Guid.NewGuid() }
            };
            var testEntityRepositoryMock = new Mock<IRepository<LazyLoadedEntity>>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var LazyLoader = new LazyLoader(unitOfWorkMock.Object);
            var lazyLoadingEntity = new LazyLoadingEntity { Id = Guid.NewGuid(), TestEntityId = testCollection[1].Id };
            testEntityRepositoryMock.Setup(x => x.FindById(testCollection[0].Id)).Returns(testCollection[0]);
            testEntityRepositoryMock.Setup(x => x.FindById(testCollection[1].Id)).Returns(testCollection[1]);
            testEntityRepositoryMock.Setup(x => x.FindById(testCollection[2].Id)).Returns(testCollection[2]);
            testEntityRepositoryMock.Setup(x => x.Where("{LazyLoadingEntityId} = @Id", lazyLoadingEntity, string.Empty)).Returns(() =>
                {
                    var newList = new List<LazyLoadedEntity>();
                    for (int i = 0; i < testCollection.Count; i++)
                    {
                        testCollection[i].LazyLoadingEntityId = lazyLoadingEntity.Id;
                        newList.Add(testCollection[i]);
                    }
                    return newList;
                });
            unitOfWorkMock.Setup(x => x.Repo<LazyLoadedEntity>()).Returns(testEntityRepositoryMock.Object);

            // Test Single
            LazyLoader.Init(lazyLoadingEntity);
            Assert.That(lazyLoadingEntity.TestEntity, Is.Not.Null);
            Assert.That(lazyLoadingEntity.TestEntity.IsValueCreated, Is.False);
            testEntityRepositoryMock.Verify(x => x.FindById(testCollection[1].Id), Times.Never());
            var loadedEntity = lazyLoadingEntity.TestEntity.Value;
            testEntityRepositoryMock.Verify(x => x.FindById(testCollection[1].Id), Times.Once());
            Assert.That(lazyLoadingEntity.TestEntity.IsValueCreated, Is.True);
            Assert.That(loadedEntity, Is.Not.Null);
            Assert.That(loadedEntity.Id, Is.EqualTo(testCollection[1].Id));

            // Test List
            Assert.That(lazyLoadingEntity.TestEntities, Is.Not.Null);
            Assert.That(lazyLoadingEntity.TestEntities.IsValueCreated, Is.False);
            testEntityRepositoryMock.Verify(x => x.Where("{LazyLoadingEntityId} = @Id", lazyLoadingEntity, string.Empty), Times.Never());
            var loadedEntities = lazyLoadingEntity.TestEntities.Value;
            testEntityRepositoryMock.Verify(x => x.Where("{LazyLoadingEntityId} = @Id", lazyLoadingEntity, string.Empty), Times.Once());
            Assert.That(lazyLoadingEntity.TestEntities.IsValueCreated, Is.True);
            Assert.That(loadedEntities, Is.Not.Null);
            Assert.That(loadedEntities.Count, Is.EqualTo(3));
            Assert.That(loadedEntities[0], Is.SameAs(testCollection[0]));
            Assert.That(loadedEntities[1], Is.SameAs(testCollection[1]));
            Assert.That(loadedEntities[2], Is.SameAs(testCollection[2]));
        }
    }
}
