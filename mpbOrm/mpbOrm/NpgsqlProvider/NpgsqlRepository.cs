// The MIT License (MIT)
//
// Copyright (c) 2014 Bernhard Johannessen
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the folloawing conditions:
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

namespace mpbOrm.NpgsqlProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Data;
    using Npgsql;
    using Dapper;

    public class NpgsqlRepository<TEntity> : RepositoryBase<TEntity>, IRepository<TEntity>
        where TEntity : IEntity
    {
        private string table;
        private string[] fields;

        public NpgsqlRepository(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            var map = unitOfWork.Map<TEntity>();
            table = map.TableName;
            fields = map.ColumnNames();
        }

        public TEntity FindById(Guid id)
        {
            var entity = unitOfWork.EntityCache.Map<TEntity>().Get(id);
            if (entity == null)
            {
                entity = this.QuerySingle<TEntity>(
                    string.Format(
                        "SELECT {0} FROM {1} WHERE Id = @Id",
                        string.Join(", ", from f in fields select table + "." + f),
                        table),
                    new { Id = id });
            }
            return entity;
        }

        public TEntity Single(string filter = "", object param = null)
        {
            return ((List<TEntity>)this.Where(filter, param)).SingleOrDefault();
        }

        public List<TEntity> Where(string filter = "", object param = null, string orderBy = "")
        {
            return this.Query<TEntity>(
                this.SelectBuilder(filter, orderBy).ToString(),
                param);
        }

        public PagedResult<TEntity> Paged(string filter = "", object param = null, int page = 1, int pageSize = 10, string orderBy = "")
        {
            var builder = this.SelectBuilder(filter, orderBy);
            builder.Append(
                string.Format(
                    " OFFSET {0}",
                    (page - 1) * pageSize));
            builder.Append(
                string.Format(
                    " LIMIT {0}",
                    pageSize));
            var result = this.Query<TEntity>(builder.ToString(), param);
            var countBuilder = new StringBuilder();
            countBuilder.Append(
                string.Format(
                    "SELECT CAST(COUNT(*) AS INT) FROM {0}",
                    table));
            if (!string.IsNullOrEmpty(filter))
                countBuilder.Append(
                    string.Format(
                        " WHERE {0}",
                        filter));
            var total = this.QueryNonEntitySingle<int>(countBuilder.ToString(), param);
            var pagedResult = new PagedResult<TEntity>
            {
                Page = page,
                PageSize = pageSize,
            };
            pagedResult.AddRange(result);
            return pagedResult;
        }

        public void Add(TEntity entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();
            this.Execute(
                string.Format(
                    "INSERT INTO {0}({1}) VALUES({2})",
                    table,
                    string.Join(", ", fields),
                    string.Join(", ", from f in fields select "@" + f)),
                entity);
            this.Load(entity);
        }

        public void Update(TEntity entity)
        {
            this.Execute(
                string.Format(
                    "UPDATE {0} SET {1} WHERE Id = @Id",
                    table,
                    string.Join(", ", from f in fields where f.ToLower() != "id" select string.Format("{0} = @{1}", f))),
                entity);
        }

        public void Remove(TEntity entity)
        {
            this.Execute(
                string.Format(
                    "DELETE FROM {0} WHERE Id = @Id",
                    table),
                entity);
            unitOfWork.EntityCache.Map<TEntity>().Remove(entity.Id);
        }

        protected StringBuilder SelectBuilder(string filter = "", string orderBy = "")
        {
            var builder = new StringBuilder();
            builder.Append(
                string.Format(
                    "SELECT {0} FROM {1}",
                    string.Join(", ", from f in fields select table + "." + f),
                    table));
            if (!string.IsNullOrEmpty(filter))
                builder.Append(
                    string.Format(
                        " WHERE {0}",
                        filter));
            if (!string.IsNullOrEmpty(orderBy))
                builder.Append(
                    string.Format(
                        " ORDER BY {0}",
                        orderBy));
            return builder;
        }
    }
}
