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
    using mpbOrm.Tests.TestClasses;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class IdentityMapTests
    {
        [Test]
        public void CanAddRetriveAndRemoveEntities()
        {
            var identityMap = new IdentityMap<TestEntity>();
            var testEntity1 = new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Number = 123
            };
            var testEntity2 = new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test2",
                Number = 321
            };
            Assert.DoesNotThrow(() =>
                {
                    identityMap.Add(testEntity1);
                });
            Assert.DoesNotThrow(() =>
                {
                    identityMap.Add(testEntity2);
                });
            TestEntity testEntity1Reference = null;
            TestEntity testEntity2Reference = null; 
            Assert.DoesNotThrow(() =>
                {
                    testEntity1Reference = identityMap.Get(testEntity1.Id);
                });
            Assert.That(testEntity1Reference, Is.Not.Null);
            Assert.That(testEntity1Reference, Is.SameAs(testEntity1));
            Assert.DoesNotThrow(() =>
                {
                    testEntity2Reference = identityMap.Get(testEntity2.Id);
                });
            Assert.That(testEntity2Reference, Is.Not.Null);
            Assert.That(testEntity2Reference, Is.SameAs(testEntity2));
            Assert.DoesNotThrow(() =>
                {
                    identityMap.Remove(testEntity1.Id);
                });
            TestEntity testEntity1Reference2 = null;
            Assert.DoesNotThrow(() =>
                {
                    testEntity1Reference2 = identityMap.Get(testEntity1.Id);
                });
            Assert.That(testEntity1Reference2, Is.Null);
        }

        [Test]
        public void ReturnsNullIfEntityNotInMap()
        {
            var identityMap = new IdentityMap<TestEntity>();
            var testEntity1 = new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Number = 123
            };
            var testEntity2 = new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test2",
                Number = 321
            };
            identityMap.Add(testEntity1);
            var testEntity2Reference = identityMap.Get(testEntity2.Id);
            Assert.That(testEntity2Reference, Is.Null);
        }
    }
}
