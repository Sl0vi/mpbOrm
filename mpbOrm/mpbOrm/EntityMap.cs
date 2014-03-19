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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;

    public class EntityMap<TEntity>
            where TEntity : IEntity
    {
        private string tableName = null;

        public EntityMapContainer Container { get; set; }

        public Dictionary<PropertyInfo, string> Columns { get; set; }

        public EntityMap(EntityMapContainer container)
        {
            this.Container = container;
            this.Columns = new Dictionary<PropertyInfo, string>();
        }

        public string TableName
        {
            get
            {
                return string.IsNullOrEmpty(tableName) ? typeof(TEntity).Name : tableName;
            }
            set
            {
                tableName = value;
            }
        }
        
        public EntityMap<TEntity> MapProperty<TType, TValue>(Expression<Func<TType, TValue>> expression, string columnName)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format("Expression {0} does not refer to a property", expression), "expression");
            var propertyInfo = member.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException(string.Format("Expression {0} refers to a field, not a property", expression));
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentException("columnName cannot be empty", "columnName");
            this.Columns[propertyInfo] = columnName;
            return this;
        }

        public string ColumnName<TType, TValue>(Expression<Func<TType, TValue>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format("Expression {0} does not refer to a property", expression), "expression");
            var propertyInfo = member.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException(string.Format("Expression {0} refers to a field, not a property", expression));
            return this.ColumnName(propertyInfo);
        }

        public string ColumnName(PropertyInfo propertyInfo)
        {
            string name;
            if (this.Columns.TryGetValue(propertyInfo, out name))
                return name;
            return propertyInfo.Name;
        }

        public string[] ColumnNames(bool prependTable = false)
        {
            var validTypes = new Type[]
            {
                typeof(Guid),
                typeof(byte),
                typeof(byte[]),
                typeof(int),
                typeof(long),
                typeof(decimal),
                typeof(float),
                typeof(double),
                typeof(bool),
                typeof(string),
                typeof(DateTime),
                typeof(TimeSpan),
                typeof(IPAddress)
            };
            var properties = typeof(TEntity).GetProperties();
            var columns = new List<string>();
            foreach (var property in properties)
            {
                if (validTypes.Contains(property.PropertyType))
                {
                    if (prependTable)
                        columns.Add(string.Format("{0}.{1}", this.TableName, this.ColumnName(property)));
                    else
                        columns.Add(this.ColumnName(property));
                }
            }
            return columns.ToArray();
        }
    }
}
