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
    public class EntityMapTests
    {
        [Test]
        public void ReturnsClassNameIfTableNameNotExplicitlySet()
        {
            var entityMap = new EntityMap<TestEntity>();
            Assert.That(entityMap.TableName, Is.EqualTo("TestEntity"));
        }

        [Test]
        public void ReturnsExplicitlySetTableName()
        {
            var entityMap = new EntityMap<TestEntity>();
            entityMap.TableName = "TestTableName";
            Assert.That(entityMap.TableName, Is.EqualTo("TestTableName"));
        }

        [Test]
        public void CanMapColumnName()
        {
            var entityMap = new EntityMap<TestEntity>();
            Assert.DoesNotThrow(() =>
                {
                    entityMap.MapProperty(p => p.Id, "TestId");
                });
            Assert.That(entityMap.ColumnName(p => p.Id), Is.EqualTo("TestId"));
        }

        [Test]
        public void ReturnsPropertyNameFromExpression()
        {
            var entityMap = new EntityMap<TestEntity>();
            Assert.That(entityMap.ColumnName(p => p.Id), Is.EqualTo("Id"));
        }

        [Test]
        public void ReturnsMappedColumnNameFromExpression()
        {
            var entityMap = new EntityMap<TestEntity>();
            entityMap.MapProperty(p => p.Id, "Test");
            Assert.That(entityMap.ColumnName(p => p.Id), Is.EqualTo("Test"));
        }

        [Test]
        public void ReturnsPropertyNameFromPropertyInfo()
        {
            var entityMap = new EntityMap<TestEntity>();
            var propertyInfo = typeof(TestEntity).GetProperty("Id");
            Assert.That(entityMap.ColumnName(propertyInfo), Is.EqualTo("Id"));
        }

        [Test]
        public void ReturnsMappedColumnNameFromPropertyInfo()
        {
            var entityMap = new EntityMap<TestEntity>();
            entityMap.MapProperty(p => p.Id, "Test");
            var propertyInfo = typeof(TestEntity).GetProperty("Id");
            Assert.That(entityMap.ColumnName(propertyInfo), Is.EqualTo("Test"));
        }

        [Test]
        public void ColumnNamesReturnsColumnNamesForPropertiesOnly()
        {
            var entityMap = new EntityMap<TestEntity>();
            var columnNames = entityMap.ColumnNames();
            Assert.That(columnNames.Length, Is.EqualTo(3));
            Assert.That(columnNames, Is.Unique);
            Assert.That(columnNames, Contains.Item("Id"));
            Assert.That(columnNames, Contains.Item("Name"));
            Assert.That(columnNames, Contains.Item("Number"));
            Assert.That(columnNames, Has.No.Member("stringField"));
            Assert.That(columnNames, Has.No.Member("intField"));
            Assert.That(columnNames, Has.No.Member("TestMethod"));
        }

        [Test]
        public void ColumnNamesReturnsMappedColumnNames()
        {
            var entityMap = new EntityMap<TestEntity>();
            entityMap.MapProperty(p => p.Id, "TestId");
            entityMap.MapProperty(p => p.Name, "TestName");
            entityMap.MapProperty(p => p.Number, "TestNumber");
            var columnNames = entityMap.ColumnNames();
            Assert.That(columnNames.Length, Is.EqualTo(3));
            Assert.That(columnNames, Is.Unique);
            Assert.That(columnNames, Contains.Item("TestId"));
            Assert.That(columnNames, Contains.Item("TestName"));
            Assert.That(columnNames, Contains.Item("TestNumber"));
        }

        [Test]
        public void ColumnNamesCanPrependClassName()
        {
            var entityMap = new EntityMap<TestEntity>();
            var columnNames = entityMap.ColumnNames(prependTable: true);
            Assert.That(columnNames.Length, Is.EqualTo(3));
            Assert.That(columnNames, Is.Unique);
            Assert.That(columnNames, Contains.Item("TestEntity.Id"));
            Assert.That(columnNames, Contains.Item("TestEntity.Name"));
            Assert.That(columnNames, Contains.Item("TestEntity.Number"));
        }

        [Test]
        public void ColumnNamesPrependsTableNameIfSet()
        {
            var entityMap = new EntityMap<TestEntity>();
            entityMap.TableName = "TableName";
            var columnNames = entityMap.ColumnNames(prependTable: true);
            Assert.That(columnNames.Length, Is.EqualTo(3));
            Assert.That(columnNames, Is.Unique);
            Assert.That(columnNames, Contains.Item("TableName.Id"));
            Assert.That(columnNames, Contains.Item("TableName.Name"));
            Assert.That(columnNames, Contains.Item("TableName.Number"));
        }

        [Test]
        public void ThrowsIfColumnNameEmpty()
        {
            var entityMap = new EntityMap<TestEntity>();
            Assert.Throws<ArgumentException>(() =>
                {
                    entityMap.MapProperty(p => p.Id, null);
                });
            Assert.Throws<ArgumentException>(() =>
                {
                    entityMap.MapProperty(p => p.Id, "");
                });
        }

        [Test]
        public void CannotMapFields()
        {
            var entityMap = new EntityMap<TestEntity>();
            Assert.Throws<ArgumentException>(() =>
                {
                    entityMap.MapProperty(p => p.intField, "Test");
                });
            Assert.Throws<ArgumentException>(() =>
                {
                    entityMap.MapProperty(p => p.stringField, "Test");
                });
        }
    }
}
